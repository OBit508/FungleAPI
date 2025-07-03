using AmongUs.GameOptions;
using FungleAPI.Patches;
using FungleAPI.Role.Teams;
using FungleAPI.Roles;
using HarmonyLib;
using Il2CppSystem.Text;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

namespace FungleAPI.Role
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
                if (role.CachedConfiguration.ShowTeamColor)
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
            if (role != null && role.CachedConfiguration.HintType != RoleTaskHintType.Normal)
            {
                return false;
            }
            return true;
        }
    }
}
