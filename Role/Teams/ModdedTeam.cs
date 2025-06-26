using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FungleAPI.LoadMod;
using FungleAPI.Patches;
using FungleAPI.Translation;
using UnityEngine;

namespace FungleAPI.Role.Teams
{
    public class ModdedTeam
    {
        public static ModdedTeam Crewmates;
        public static ModdedTeam Impostors;
        public static ModdedTeam Neutrals;
        private static int Ids = 20;
        public static ModdedTeam RegisterTeam(Type type)
        {
            ModdedTeam team = (ModdedTeam)Activator.CreateInstance(type);
            team.TeamPlugin.Teams.Add(team);
            team.WinReason = (GameOverReason)Ids;
            Ids++;
            return team;
        }
        public GameOverReason WinReason;
        public ModPlugin TeamPlugin => ModPlugin.GetModPlugin(GetType().Assembly);
        public virtual Color TeamColor => Palette.CrewmateBlue;
        public virtual StringNames TeamName => StringNames.None;
        public virtual bool FriendlyFire => true;
        public virtual bool KnowMembers => false;
    }
}
