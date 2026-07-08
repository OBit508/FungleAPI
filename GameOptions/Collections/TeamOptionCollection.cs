using FungleAPI.Api;
using FungleAPI.GameOptions.Patches;
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
        public const int TeamOptionVersion = 3;

        public int LocalTeamCount;
        public int NonHostTeamCount;

        public int LocalTeamPriority;
        public int NonHostTeamPriority;

        public ModdedTeam Team;

        public void SetLocal(int count, int priority)
        {
            if (LocalTeamCount != count) { LocalTeamCount = count; Dirty = true; }
            if (LocalTeamPriority != priority) { LocalTeamPriority = priority; Dirty = true; }
        }
        public override void WriteLocalOptions(BinaryWriter binaryWriter)
        {
            binaryWriter.Write(TeamOptionVersion);

            binaryWriter.Write(LocalTeamCount);
            binaryWriter.Write(LocalTeamPriority);

            base.WriteLocalOptions(binaryWriter);
        }
        public override void ReadLocalOptions(BinaryReader binaryReader)
        {
            try
            {
                int teamOptionVersion = binaryReader.ReadInt32();
                if (teamOptionVersion < TeamOptionVersion)
                {
                    FungleApiPlugin.Instance.Log.LogWarning($"Different version of the Team Option Collection from {FilePath} founded, loading default.");
                    SetAsDefault(true);
                    return;
                }

                LocalTeamCount = binaryReader.ReadInt32();
                LocalTeamPriority = binaryReader.ReadInt32();

                base.ReadLocalOptions(binaryReader);
            }
            catch (Exception ex)
            {
                FungleApiPlugin.Instance.Log.LogError($"Failed to read Team Option Collection from {FilePath}, loading default.\nMessage: {ex.Message}");
                SetAsDefault(true);
            }
        }
        public override void SetAsDefault(bool amHost)
        {
            if (amHost)
            {
                LocalTeamCount = Team.DefaultCount;
                LocalTeamPriority = Team.DefaultPriority;
            }
            else
            {
                NonHostTeamCount = Team.DefaultCount;
                NonHostTeamPriority = Team.DefaultPriority;
            }

            base.SetAsDefault(amHost);
        }
        public TeamOptionCollection(ModdedTeam moddedTeam)
        {
            Team = moddedTeam;
            FolderName = "Teams";
        }
    }
}
