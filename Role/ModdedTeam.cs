using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FungleAPI.LoadMod;
using FungleAPI.Patches;
using UnityEngine;

namespace FungleAPI.Roles
{
    public class ModdedTeam
    {
        internal ModdedTeam()
        {
        }
        public static ModdedTeam Crewmates;
        public static ModdedTeam Impostors;
        private static int Ids = 1;
        public static ModdedTeam Create(ModPlugin plugin, Color teamColor, string teamName, RoleTeamTypes BasedTeam)
        {
            ModdedTeam team = new ModdedTeam();
            team.WinReason = (GameOverReason)(Ids + 50);
            Ids++;
            team.TeamColor = teamColor;
            team.TeamName = teamName;
            team.BaseTeam = BasedTeam;
            team.TeamPlugin = plugin;
            plugin.Teams.Add(team);
            return team;
        }
        public ModPlugin TeamPlugin;
        public GameOverReason WinReason;
        public Color TeamColor;
        public string TeamName;
        public RoleTeamTypes BaseTeam;
    }
}
