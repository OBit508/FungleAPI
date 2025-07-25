using FungleAPI.Roles;
using FungleAPI.Translation;
using FungleAPI.Utilities;
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
        internal static ModdedTeam RegisterTeam(Type type, ModPlugin plugin)
        {
            ModdedTeam team = (ModdedTeam)Activator.CreateInstance(type);
            team.WinReason = CustomRoleManager.GetValidGameOver();
            plugin.BasePlugin.Log.LogInfo("Registered Team " + type.Name);
            Teams.Add(team);
            return team;
        }
        public static T GetInstance<T>() where T : ModdedTeam
        {
            foreach (ModdedTeam team in Teams)
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
        public static List<ModdedTeam> Teams = new List<ModdedTeam>();
    }
}
