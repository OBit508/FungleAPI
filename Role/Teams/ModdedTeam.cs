using FungleAPI.LoadMod;
using FungleAPI.Patches;
using FungleAPI.Roles;
using FungleAPI.Translation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace FungleAPI.Role.Teams
{
    public class ModdedTeam
    {
        public static ModdedTeam Crewmates => GetInstance<CrewmateTeam>();
        public static ModdedTeam Impostors => GetInstance<ImpostorTeam>();
        public static ModdedTeam Neutrals => GetInstance<NeutralTeam>();
        private static int Ids = 20;
        internal static ModdedTeam RegisterTeam(Type type, ModPlugin plugin)
        {
            ModdedTeam team = (ModdedTeam)Activator.CreateInstance(type);
            team.WinReason = (GameOverReason)Ids;
            Ids++;
            plugin.BasePlugin.Log.LogInfo("Registered Team " + type.Name);
            return team;
        }
        public static T GetInstance<T>() where T : ModdedTeam
        {
            foreach (ModdedTeam team in ModPlugin.GetModPlugin(typeof(T).Assembly).Teams)
            {
                if (team.GetType() == typeof(T))
                {
                    return team.SimpleCast<T>();
                }
            }
            return null;
        }
        public GameOverReason WinReason;
        public ModPlugin TeamPlugin => ModPlugin.GetModPlugin(GetType().Assembly);
        public virtual Color TeamColor => Palette.CrewmateBlue;
        public virtual StringNames TeamName => StringNames.None;
        public virtual bool FriendlyFire => true;
        public virtual bool KnowMembers => false;
    }
}
