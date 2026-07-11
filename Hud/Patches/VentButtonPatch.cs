using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FungleAPI.Role;
using FungleAPI.ModCompatibility;
using HarmonyLib;

namespace FungleAPI.Hud.Patches
{
    [HarmonyPatch(typeof(VentButton))]
    internal static class VentButtonPatch
    {
        [HarmonyPatch("SetTarget")]
        [HarmonyPrefix]
        public static bool SetTargetPrefix(VentButton __instance, Vent target)
        {
            if (!MiraCompatibility.ShouldHandleLocalRole()) return true;
            RoleConfigManager.VentConfig.SetTarget?.Invoke(target);
            return false;
        }
        [HarmonyPatch("DoClick")]
        [HarmonyPrefix]
        public static bool DoClickPrefix(VentButton __instance)
        {
            if (!MiraCompatibility.ShouldHandleLocalRole()) return true;
            RoleConfigManager.VentConfig.DoClick?.Invoke();
            return false;
        }
    }
}
