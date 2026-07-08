using HarmonyLib;
using LibCpp2IL.MachO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FungleAPI.GameModes.Patches
{
    [HarmonyPatch(typeof(ReportButton))]
    internal static class ReportButtonPatch
    {
        [HarmonyPatch(nameof(ReportButton.SetActive))]
        [HarmonyPostfix]
        public static void SetActivePostfix(ReportButton __instance)
        {
            if (!GameModeManager.GetCurrentGameMode().CanReportBodies() && !GameManager.Instance.IsHideAndSeek())
            {
                __instance.ToggleVisible(false);
                return;
            }
        }
        [HarmonyPatch(nameof(ReportButton.DoClick))]
        [HarmonyPrefix]
        public static bool DoClickPrefix()
        {
            if (!GameModeManager.GetCurrentGameMode().CanReportBodies() && !GameManager.Instance.IsHideAndSeek()) return false;
            return true;
        }
    }
}
