using AmongUs.GameOptions;
using BepInEx.Configuration;
using FungleAPI.ModCompatibility;
using FungleAPI.Networking;
using FungleAPI.Role.Utilities;
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
    [HarmonyPatch(typeof(RoleOptionsCollectionV12))]
    internal static class RoleOptionsCollectionV12Patch
    {
        [HarmonyPrefix]
        [HarmonyPatch(nameof(RoleOptionsCollectionV12.AnyRolesEnabled))]
        public static bool AnyRolesEnabledPrefix(RoleOptionsCollectionV12 __instance, ref bool __result)
        {
            foreach (Il2CppSystem.Collections.Generic.KeyValuePair<RoleTypes, RoleDataV12> keyValuePair in __instance.roles)
            {
                if (__instance.GetNumPerGame(keyValuePair.Key) > 0)
                {
                    __result = true;
                }
            }
            return false;
        }
        [HarmonyPrefix]
        [HarmonyPatch(nameof(RoleOptionsCollectionV12.GetChancePerGame))]
        public static bool GetChancePrefix(RoleTypes roleType, ref int __result)
        {
            ICustomRole role = RoleManager.Instance.GetRole(roleType).CustomRole();
            if (role != null)
            {
                __result = role.GetChance();
                return false;
            }
            return true;
        }
        [HarmonyPrefix]
        [HarmonyPatch(nameof(RoleOptionsCollectionV12.GetNumPerGame))]
        public static bool GetNumPrefix(RoleTypes roleType, ref int __result)
        {
            ICustomRole role = RoleManager.Instance.GetRole(roleType).CustomRole();
            if (role != null)
            {
                __result = role.GetCount();
                return false;
            }
            return true;
        }
    }
}
