using FungleAPI.Components;
using FungleAPI.Role;
using FungleAPI.ModCompatibility;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace FungleAPI.Hud.Patches
{
    [HarmonyPatch(typeof(KillButton))]
    internal static class KillButtonPatch
    {
        [HarmonyPatch("SetTarget")]
        [HarmonyPrefix]
        public static bool SetTargetPrefix(KillButton __instance, PlayerControl target)
        {
            if (!MiraCompatibility.ShouldHandleLocalRole()) return true;
            RoleConfigManager.KillConfig.SetTarget(target);
            return false;
        }
        [HarmonyPatch("CheckClick")]
        [HarmonyPrefix]
        public static bool CheckClickPrefix(KillButton __instance, PlayerControl target)
        {
            if (!MiraCompatibility.ShouldHandleLocalRole()) return true;
            RoleConfigManager.KillConfig.CheckClick(target);
            return false;
        }
        [HarmonyPatch("DoClick")]
        [HarmonyPrefix]
        public static bool DoClickPrefix(KillButton __instance)
        {
            if (!MiraCompatibility.ShouldHandleLocalRole()) return true;
            RoleConfigManager.KillConfig.DoClick();
            return false;
        }
        [HarmonyPatch("ResetKillButton")]
        [HarmonyPostfix]
        public static void ResetKillButtonPostfix(KillButton __instance)
        {

        }
    }
}
