using FungleAPI.GameOptions.Patches;
using FungleAPI.PluginLoading;
using FungleAPI.Role;
using FungleAPI.Utilities;
using Hazel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.UIElements.UIR;

namespace FungleAPI.GameOptions.Collections
{
    public class RoleOptionCollection : OptionCollection
    {
        public const int RoleOptionVersion = 2;

        public int LocalRoleCount;
        public int NonHostRoleCount;

        public int LocalRoleChance;
        public int NonHostRoleChance;

        public int RoleCount => AmongUsClient.Instance.AmHost ? LocalRoleCount : NonHostRoleCount;
        public int RoleChance => AmongUsClient.Instance.AmHost ? LocalRoleChance : NonHostRoleChance;

        public ICustomRole Role;

        public void SetLocal(int count, int chance)
        {
            if (LocalRoleCount != count) { LocalRoleCount = count; Dirty = true; }
            if (LocalRoleChance != chance) { LocalRoleChance = chance; Dirty = true; }
        }
        public override void Initialize(Type type, ModPlugin modPlugin)
        {
            Plugin = modPlugin;
            CollectionId = $"{type.Name}.{type.GetShortUniqueId()}";
            FilePath = Path.Combine(FileManager.GetFolder(modPlugin, FolderType.Roles), $"{CollectionId}.funglecfg");
            foreach (IModdedOption moddedOption in OptionManager.GetAndInitializeModdedOptions(type, modPlugin))
            {
                moddedOption.SetOnValueChance((bool changed) => { if (changed) { Dirty = true; if (LobbyViewSettingsPanePatch.Tab != null && LobbyViewSettingsPanePatch.Tab.Plugin == modPlugin) { LobbyViewSettingsPanePatch.Tab.RefreshViewTab?.Invoke(); } } });
                OptionManager.AllOptions.Add(moddedOption.OptionId, moddedOption);
                Options.Add(moddedOption.OptionId, moddedOption);
            }

            modPlugin.OptionCollections.Add(this);
            OptionManager.OptionCollections.Add(this);

            ReadLocalOptions();
        }
        public override void WriteLocalOptions()
        {
            using (FileStream fileStream = new FileStream(FilePath, FileMode.Create, FileAccess.Write))
            {
                using (BinaryWriter binaryWriter = new BinaryWriter(fileStream))
                {
                    binaryWriter.Write(RoleOptionVersion);

                    binaryWriter.Write(LocalRoleCount);
                    binaryWriter.Write(LocalRoleChance);

                    binaryWriter.Write(Options.Count);
                    foreach (IModdedOption moddedOption in Options.Values)
                    {
                        binaryWriter.Write(moddedOption.StringOptionId);
                        moddedOption.WriteLocalValue(binaryWriter);
                    }

                    binaryWriter.Flush();
                    fileStream.Flush(true);
                }
            }
        }
        public override void ReadLocalOptions()
        {
            if (!File.Exists(FilePath))
            {
                SetAsDefault(true);
                return;
            }
            try
            {
                using (MemoryStream memoryStream = new MemoryStream(File.ReadAllBytes(FilePath)))
                {
                    using (BinaryReader binaryReader = new BinaryReader(memoryStream))
                    {
                        int roleOptionVersion = binaryReader.ReadInt32();
                        if (roleOptionVersion < RoleOptionVersion)
                        {
                            FungleApiPlugin.Instance.Log.LogWarning($"Newer version of the Role Option Collection from {FilePath} founded, loading and saving default.");
                            SetAsDefault(true);
                            return;
                        }

                        LocalRoleCount = binaryReader.ReadInt32();
                        LocalRoleChance = binaryReader.ReadInt32();

                        int optionCount = binaryReader.ReadInt32();
                        for (int i = 0; i < optionCount; i++)
                        {
                            string optionId = binaryReader.ReadString();
                            IModdedOption moddedOption = Options.Values.FirstOrDefault(m => m.StringOptionId == optionId);
                            if (moddedOption != null)
                            {
                                moddedOption.ReadLocalValue(binaryReader);
                            }
                        }

                        SyncNonHostWithLocal();
                    }
                }
            }
            catch (Exception ex)
            {
                FungleApiPlugin.Instance.Log.LogError($"Failed to read Role Option Collection from {FilePath}, loading and saving default.\nMessage: {ex.Message}");
                SetAsDefault(true);
            }
        }
        public override void SyncNonHostWithLocal()
        {
            NonHostRoleCount = LocalRoleCount;
            NonHostRoleChance = LocalRoleChance;
            foreach (IModdedOption moddedOption in Options.Values)
            {
                moddedOption.SyncNonHostWithLocal();
            }
        }
        public override void SetAsDefault(bool amHost)
        {
            if (amHost)
            {
                LocalRoleCount = Role.Configuration.DefaultCount;
                LocalRoleChance = Role.Configuration.DefaultChance;
            }
            else
            {
                NonHostRoleCount = Role.Configuration.DefaultCount;
                NonHostRoleChance = Role.Configuration.DefaultChance;
            }

            foreach (IModdedOption moddedOption in Options.Values)
            {
                moddedOption.SetValue(moddedOption.DefaultValue, amHost);
            }

            if (amHost)
            {
                SyncNonHostWithLocal();
            }
        }
        public RoleOptionCollection(ICustomRole customRole)
        {
            Role = customRole;
        }
    }
}
