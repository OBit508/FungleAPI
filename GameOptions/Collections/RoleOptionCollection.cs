using FungleAPI.PluginLoading;
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
        public const int RoleOptionVersion = 1;

        public int LocalRoleCount;
        public int NonHostRoleCount;

        public int LocalRoleChance;
        public int NonHostRoleChance;

        public int RoleCount => AmongUsClient.Instance.AmHost ? LocalRoleCount : NonHostRoleCount;
        public int RoleChance => AmongUsClient.Instance.AmHost ? LocalRoleChance : NonHostRoleChance;

        public void SetLocal(int count, int chance)
        {
            if (LocalRoleCount != count) { LocalRoleCount = count; Dirty = true; }
            if (LocalRoleChance != chance) { LocalRoleChance = chance; Dirty = true; }
        }
        public override void Initialize(Type type, ModPlugin modPlugin)
        {
            CollectionId = $"{type.Name}.{type.GetShortUniqueId()}";
            FilePath = Path.Combine(FileManager.GetFolder(modPlugin, FolderType.Roles), $"{CollectionId}.funglecfg");
            foreach (IModdedOption moddedOption in OptionManager.GetAndInitializeModdedOptions(type))
            {
                moddedOption.SetOnValueChance((bool changed) => { if (changed) { Dirty = true; } });
                OptionManager.AllOptions.Add(moddedOption.OptionId, moddedOption);
                Options.Add(moddedOption.OptionId, moddedOption);
            }
            ReadLocalOptions();
        }
        public override void Serialize(MessageWriter messageWriter, bool includeOption)
        {
            messageWriter.WritePacked(LocalRoleCount);
            messageWriter.WritePacked(LocalRoleChance);
            if (includeOption)
            {
                messageWriter.WritePacked(Options.Count);
                foreach (IModdedOption moddedOption in Options.Values)
                {
                    messageWriter.Write(moddedOption.OptionId);
                    moddedOption.Serialize(messageWriter);
                }
            }
        }
        public override void Deserialize(MessageReader messageReader, bool includeOption)
        {
            NonHostRoleCount = messageReader.ReadPackedInt32();
            NonHostRoleChance = messageReader.ReadPackedInt32();
            if (includeOption)
            {
                int optionCount = messageReader.ReadPackedInt32();
                for (int i = 0; i < optionCount; i++)
                {
                    string optionId = messageReader.ReadString();
                    if (Options.TryGetValue(optionId, out IModdedOption moddedOption))
                    {
                        moddedOption.Deserialize(messageReader);
                    }
                }
            }
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
                        binaryWriter.Write(moddedOption.OptionId);
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
                SetAsDefault();
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
                            FungleAPIPlugin.Instance.Log.LogWarning($"Newer version of the Role Option Collection from {FilePath} founded, loading and saving default.");
                            SetAsDefault();
                            return;
                        }

                        LocalRoleCount = binaryReader.ReadInt32();
                        LocalRoleChance = binaryReader.ReadInt32();

                        int optionCount = binaryReader.ReadInt32();
                        for (int i = 0; i < optionCount; i++)
                        {
                            string optionId = binaryReader.ReadString();
                            if (Options.TryGetValue(optionId, out IModdedOption moddedOption))
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
                FungleAPIPlugin.Instance.Log.LogError($"Failed to read Role Option Collection from {FilePath}, loading and saving default.\nMessage: {ex.Message}");
                SetAsDefault();
            }
        }
        public void SyncNonHostWithLocal()
        {
            NonHostRoleCount = LocalRoleCount;
            NonHostRoleChance = LocalRoleChance;
            foreach (IModdedOption moddedOption in Options.Values)
            {
                moddedOption.SyncNonHostWithLocal();
            }
        }
        public void SetAsDefault()
        {
            LocalRoleCount = 0;
            LocalRoleChance = 0;
            foreach (IModdedOption moddedOption in Options.Values)
            {
                moddedOption.SetValue(moddedOption.DefaultValue, true);
            }
            WriteLocalOptions();
            SyncNonHostWithLocal();
        }
    }
}
