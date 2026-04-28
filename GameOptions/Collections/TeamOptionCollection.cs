using FungleAPI.PluginLoading;
using FungleAPI.Teams;
using FungleAPI.Utilities;
using Hazel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace FungleAPI.GameOptions.Collections
{
    public class TeamOptionCollection : OptionCollection
    {
        public const int TeamOptionVersion = 1;

        public int LocalTeamCount;
        public int NonHostTeamCount;

        public int LocalTeamPriority;
        public int NonHostTeamPriority;

        public ModdedTeam Team;

        public int TeamCount => AmongUsClient.Instance.AmHost ? LocalTeamCount : NonHostTeamCount;
        public int TeamPriority => AmongUsClient.Instance.AmHost ? LocalTeamPriority : NonHostTeamPriority;

        public void SetLocal(int count, int priority)
        {
            if (LocalTeamCount != count) { LocalTeamCount = count; Dirty = true; }
            if (LocalTeamPriority != priority) { LocalTeamPriority = priority; Dirty = true; }
        }
        public override void Initialize(Type type, ModPlugin modPlugin)
        {
            CollectionId = $"{type.Name}.{type.GetShortUniqueId()}";
            FilePath = Path.Combine(FileManager.GetFolder(modPlugin, FolderType.Teams), $"{CollectionId}.funglecfg");
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
            messageWriter.WritePacked(LocalTeamCount);
            messageWriter.WritePacked(LocalTeamPriority);
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
            NonHostTeamCount = messageReader.ReadPackedInt32();
            NonHostTeamPriority = messageReader.ReadPackedInt32();
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
                    binaryWriter.Write(TeamOptionVersion);

                    binaryWriter.Write(LocalTeamCount);
                    binaryWriter.Write(LocalTeamPriority);

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
                        int teamOptionVersion = binaryReader.ReadInt32();
                        if (teamOptionVersion < TeamOptionVersion)
                        {
                            FungleAPIPlugin.Instance.Log.LogWarning($"Newer version of the Team Option Collection from {FilePath} founded, loading and saving default.");
                            SetAsDefault();
                            return;
                        }

                        LocalTeamCount = binaryReader.ReadInt32();
                        LocalTeamPriority = binaryReader.ReadInt32();

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
                FungleAPIPlugin.Instance.Log.LogError($"Failed to read Team Option Collection from {FilePath}, loading and saving default.\nMessage: {ex.Message}");
                SetAsDefault();
            }
        }
        public void SyncNonHostWithLocal()
        {
            NonHostTeamCount = LocalTeamCount;
            NonHostTeamPriority = LocalTeamPriority;
            foreach (IModdedOption moddedOption in Options.Values)
            {
                moddedOption.SyncNonHostWithLocal();
            }
        }
        public void SetAsDefault()
        {
            LocalTeamCount = Mathf.Clamp(Team.DefaultCount, 0, 1000);
            LocalTeamPriority = Team.GetType() == typeof(CrewmateTeam) ? -1 : Mathf.Clamp(Team.DefaultPriority, 0, 1000);
            foreach (IModdedOption moddedOption in Options.Values)
            {
                moddedOption.SetValue(moddedOption.DefaultValue, true);
            }
            WriteLocalOptions();
            SyncNonHostWithLocal();
        }
        public TeamOptionCollection(ModdedTeam moddedTeam)
        {
            Team = moddedTeam;
        }
    }
}
