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
    [HarmonyPatch(typeof(ReportButton))]
    internal static class ReportButtonPatch
    {
        [HarmonyPatch("SetActive")]
        [HarmonyPrefix]
        public static bool SetActivePrefix(ReportButton __instance, bool isActive)
        {
            if (!MiraCompatibility.ShouldHandleLocalRole()) return true;
            RoleConfigManager.ReportConfig.SetActive?.Invoke(isActive);
            return false;
        }
        [HarmonyPatch("DoClick")]
        [HarmonyPrefix]
        public static bool DoClickPrefix(ReportButton __instance)
        {
            if (!MiraCompatibility.ShouldHandleLocalRole()) return true;
            RoleConfigManager.ReportConfig.DoClick?.Invoke();
            return false;
        }
    }
}
