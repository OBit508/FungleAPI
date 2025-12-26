using FungleAPI.Components;
using FungleAPI.Role;
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
        public static bool SetTargetPrefix(KillButton __instance, [HarmonyArgument(0)] PlayerControl target)
        {
            CustomRoleManager.CurrentKillConfig.SetTarget?.Invoke(target);
            return false;
        }
        [HarmonyPatch("CheckClick")]
        [HarmonyPrefix]
        public static bool CheckClickPrefix(KillButton __instance, [HarmonyArgument(0)] PlayerControl target)
        {
            CustomRoleManager.CurrentKillConfig.CheckClick?.Invoke(target);
            return false;
        }
        [HarmonyPatch("DoClick")]
        [HarmonyPrefix]
        public static bool DoClickPrefix(KillButton __instance)
        {
            CustomRoleManager.CurrentKillConfig.DoClick?.Invoke();
            return false;
        }
        [HarmonyPatch("ResetKillButton")]
        [HarmonyPostfix]
        public static void ResetKillButtonPostfix(KillButton __instance)
        {

        }
    }
}
