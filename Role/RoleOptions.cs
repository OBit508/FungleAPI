using AmongUs.GameOptions;
using BepInEx.Configuration;
using FungleAPI;
using FungleAPI.Patches;
using FungleAPI.Roles;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using xCloud;

namespace FungleAPI.Role
{
    [HarmonyPatch(typeof(RoleOptionsCollectionV08))]
    internal static class RoleOptions
    {
        [HarmonyPrefix]
        [HarmonyPatch("GetChancePerGame")]
        public static bool GetChancePrefix([HarmonyArgument(0)] RoleTypes roleType, ref int __result)
        {
            ICustomRole role = CustomRoleManager.GetRole(roleType);
            if (role != null)
            {
                __result = role.RoleChance.Value;
                return false;
            }
            return true;
        }
        [HarmonyPrefix]
        [HarmonyPatch("GetNumPerGame")]
        public static bool GetNumPrefix([HarmonyArgument(0)] RoleTypes roleType, ref int __result)
        {
            ICustomRole role = CustomRoleManager.GetRole(roleType);
            if (role != null)
            {
                __result = role.RoleCount.Value;
                return false;
            }
            return true;
        }
    }
}
