using Epic.OnlineServices;
using FungleAPI.Configuration.Attributes;
using FungleAPI.Networking;
using FungleAPI.PluginLoading;
using FungleAPI.Role;
using FungleAPI.Role.Teams;
using FungleAPI.Utilities;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace FungleAPI.Configuration
{
    public static class ConfigurationManager
    {
        public static Dictionary<MethodBase, ModdedOption> Configs = new Dictionary<MethodBase, ModdedOption>();
        internal static List<RoleCountAndChance> RoleCountsAndChances = new List<RoleCountAndChance>();
        internal static List<TeamCountAndPriority> TeamCountAndPriorities = new List<TeamCountAndPriority>();
        public static bool GetPrefix(MethodBase __originalMethod, ref float __result)
        {
            __result = float.Parse(Configs[__originalMethod].GetValue());
            return false;
        }
        public static bool GetPrefix(MethodBase __originalMethod, ref int __result)
        {
            __result = int.Parse(Configs[__originalMethod].GetValue());
            return false;
        }
        public static bool GetPrefix(MethodBase __originalMethod, ref bool __result)
        {
            __result = bool.Parse(Configs[__originalMethod].GetValue());
            return false;
        }
        public static bool GetPrefix(MethodBase __originalMethod, ref string __result)
        {
            __result = Configs[__originalMethod].GetValue();
            return false;
        }
        public static void InitializeRoleCountAndChances(Type roleType, ModPlugin plugin)
        {
            RoleCountAndChance roleCountAndChance = ICustomRole.Save[roleType].CountAndChance.Value;
            roleCountAndChance.Initialize(plugin.BasePlugin.Config, plugin.ModName + " - " + roleType.FullName);
            RoleCountsAndChances.Add(roleCountAndChance);
        }
        public static void InitializeTeamCountAndPriority(ModdedTeam team, ModPlugin plugin)
        {
            team.CountAndPriority = new TeamCountAndPriority();
            team.CountAndPriority.Initialize(plugin.BasePlugin.Config, team, plugin.ModName + " - " + team.GetType().FullName);
            TeamCountAndPriorities.Add(team.CountAndPriority);
        }
    }
}
