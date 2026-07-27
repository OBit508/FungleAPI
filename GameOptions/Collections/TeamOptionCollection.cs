using BepInEx.Configuration;
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
        public ConfigEntry<byte> LocalTeamCount;
        public byte NonHostTeamCount;

        public ConfigEntry<byte> LocalTeamPriority;
        public byte NonHostTeamPriority;

        public ModdedTeam Team;

        public void SetLocal(byte count, byte priority)
        {
            LocalTeamCount.Value = count;
            LocalTeamPriority.Value = priority;
        }
        public override void LoadOptions(ConfigFile configFile, string collectionId)
        {
            LocalTeamCount = configFile.Bind(collectionId, "TeamCount", Team.DefaultCount);
            LocalTeamPriority = configFile.Bind(collectionId, "TeamPriority", Team.DefaultPriority);
            base.LoadOptions(configFile, collectionId);
        }
        public override void SetAsDefault(bool amHost)
        {
            if (amHost && LocalTeamCount != null && LocalTeamPriority != null)
            {
                LocalTeamCount.Value = Team.DefaultCount;
                LocalTeamPriority.Value = Team.DefaultPriority;
            }
            else
            {
                NonHostTeamCount = Team.DefaultCount;
                NonHostTeamPriority = Team.DefaultPriority;
            }

            base.SetAsDefault(amHost);
        }
        public TeamOptionCollection(ModdedTeam moddedTeam)
            :base("Teams", moddedTeam.GetType())
        {
            Team = moddedTeam;
        }
    }
}
