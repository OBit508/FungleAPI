using BepInEx.Configuration;
using FungleAPI.Role;
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
        public static ModdedTeam Crewmates => Instance<CrewmateTeam>();
        public static ModdedTeam Impostors => Instance<ImpostorTeam>();
        public static ModdedTeam Neutrals => Instance<NeutralTeam>();
        internal static ModdedTeam RegisterTeam(Type type, ModPlugin plugin)
        {
            ModdedTeam team = (ModdedTeam)Activator.CreateInstance(type);
            team.count = plugin.BasePlugin.Config.Bind<int>(plugin.ModName + "-" + type.FullName, "Count", 0);
            plugin.BasePlugin.Log.LogInfo("Registered Team " + type.Name + " WinReason Count: " + team.WinReason.Count);
            Teams.Add(team);
            plugin.Teams.Add(team);
            return team;
        }
        public static T Instance<T>() where T : ModdedTeam
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
        public int GetCount()
        {
            return count.Value;
        }
        public void SetCount(int value)
        {
            count.Value = value;
        }
        public virtual List<GameOverReason> WinReason { get; }
        public ModPlugin TeamPlugin => ModPlugin.GetModPlugin(GetType().Assembly);
        public virtual Color TeamColor => Palette.CrewmateBlue;
        public virtual Color TeamHeaderColor => Helpers.Light(TeamColor);
        public virtual StringNames TeamName => StringNames.None;
        public virtual StringNames PluralName => StringNames.None;
        public virtual bool FriendlyFire => true;
        public virtual bool KnowMembers => false;
        public virtual int MaxCount => int.MaxValue;
        internal ConfigEntry<int> count;
        public static List<ModdedTeam> Teams = new List<ModdedTeam>();
    }
}
