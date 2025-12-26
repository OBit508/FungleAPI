using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FungleAPI.Role;
using HarmonyLib;

namespace FungleAPI.Hud.Patches
{
    [HarmonyPatch(typeof(ReportButton))]
    internal static class ReportButtonPatch
    {
        [HarmonyPatch("SetActive")]
        [HarmonyPrefix]
        public static bool SetActivePrefix(ReportButton __instance, [HarmonyArgument(0)] bool isActive)
        {
            CustomRoleManager.CurrentReportConfig.SetActive?.Invoke(isActive);
            return false;
        }
        [HarmonyPatch("DoClick")]
        [HarmonyPrefix]
        public static bool DoClickPrefix(ReportButton __instance)
        {
            CustomRoleManager.CurrentReportConfig.DoClick?.Invoke();
            return false;
        }
    }
}
