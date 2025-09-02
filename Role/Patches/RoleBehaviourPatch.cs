using AmongUs.GameOptions;
using FungleAPI.Patches;
using FungleAPI.Role.Teams;
using FungleAPI.Roles;
using HarmonyLib;
using Il2CppSystem.Text;
using InnerNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using UnityEngine;

namespace FungleAPI.Role.Patches
{
    [HarmonyPatch(typeof(RoleBehaviour))]
    public static class RoleBehaviourPatch
    {
        [HarmonyPrefix]
        [HarmonyPatch("TeamColor", MethodType.Getter)]
        public static bool GetTeamColor(RoleBehaviour __instance, ref Color __result)
        { 
            ICustomRole role = __instance.CustomRole();
            if (role != null)
            {
                if (role.Configuration.ShowTeamColor)
                {
                    __result = role.Team.TeamColor;
                }
                else
                {
                    __result = role.RoleColor;
                }
                return false;
            }
            return true;
        }
        [HarmonyPatch("AppendTaskHint", new Type[] { typeof(StringBuilder) })]
        [HarmonyPrefix]
        public static bool OnAppendTaskHint(RoleBehaviour __instance)
        {
            ICustomRole role = __instance.CustomRole();
            if (role != null && role.Configuration.HintType != RoleTaskHintType.Normal)
            {
                return false;
            }
            return true;
        }
    }
    [HarmonyPatch]
    public static class RoleBehaviourDidWinPatch
    {
        internal static List<Type> RoleBehaviourTypes { get; } = (from x in typeof(RoleBehaviour).Assembly.GetTypes() where x.IsSubclassOf(typeof(RoleBehaviour)) select x).ToList();
        public static IEnumerable<MethodBase> TargetMethods()
        {
            return from x in RoleBehaviourTypes
                   select x.GetMethod("DidWin", AccessTools.allDeclared) into m
                   where m != null
                   select m;
        }
        public static bool Prefix(RoleBehaviour __instance, [HarmonyArgument(0)] GameOverReason gameOverReason, ref bool __result)
        {
            __result = CustomRoleManager.DidWin(__instance, gameOverReason);
            return false;
        }
    }
}
