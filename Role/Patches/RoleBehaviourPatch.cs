using AmongUs.GameOptions;
using FungleAPI.Patches;
using FungleAPI.Role.Teams;
using FungleAPI.Role;
using HarmonyLib;
using Il2CppSystem.Text;
using InnerNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using UnityEngine;
using FungleAPI.Components;

namespace FungleAPI.Role.Patches
{
    [HarmonyPatch(typeof(RoleBehaviour))]
    internal static class RoleBehaviourPatch
    {
        [HarmonyPrefix]
        [HarmonyPatch("TeamColor", MethodType.Getter)]
        public static bool TeamColorPrefix(RoleBehaviour __instance, ref Color __result)
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
    }
}
