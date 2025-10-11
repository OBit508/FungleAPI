using AmongUs.GameOptions;
using BepInEx.Configuration;
using FungleAPI.Configuration;
using FungleAPI.ModCompatibility;
using FungleAPI.Networking;
using FungleAPI.Patches;
using FungleAPI.Role;
using HarmonyLib;
using Hazel;
using Il2CppInterop.Runtime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using xCloud;
using static Il2CppSystem.Linq.Expressions.Interpreter.CastInstruction.CastInstructionNoT;

namespace FungleAPI.Role.Patches
{
    [HarmonyPatch(typeof(RoleOptionsCollectionV10))]
    internal static class RoleOptionsCollectionV010Patch
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
                __result = role.CountAndChance.GetChance();
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
                __result = role.CountAndChance.GetCount();
                return false;
            }
            return true;
        }
    }
}
