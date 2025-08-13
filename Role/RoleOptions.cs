using AmongUs.GameOptions;
using BepInEx.Configuration;
using FungleAPI;
using FungleAPI.Configuration;
using FungleAPI.MCIPatches;
using FungleAPI.Patches;
using FungleAPI.Roles;
using FungleAPI.Rpc;
using HarmonyLib;
using Hazel;
using Il2CppInterop.Runtime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using xCloud;

namespace FungleAPI.Role
{
    [HarmonyPatch(typeof(RoleOptionsCollectionV09))]
    internal static class RoleOptions
    {
        [HarmonyPrefix]
        [HarmonyPatch("AnyRolesEnabled")]
        public static bool AnyRolesEnabledPrefix(RoleOptionsCollectionV09 __instance, ref bool __result)
        {
            foreach (Il2CppSystem.Collections.Generic.KeyValuePair<RoleTypes, RoleDataV09> keyValuePair in __instance.roles)
            {
                if (__instance.GetNumPerGame(keyValuePair.Key) > 0)
                {
                    __result = true;
                }
            }
            return false;
        }
        [HarmonyPrefix]
        [HarmonyPatch("GetChancePerGame")]
        public static bool GetChancePrefix([HarmonyArgument(0)] RoleTypes roleType, ref int __result)
        {
            ICustomRole role = CustomRoleManager.GetRole(roleType);
            if (role != null)
            {
                __result = role.RoleChance;
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
                __result = role.RoleCount;
                return false;
            }
            return true;
        }
    }
}
