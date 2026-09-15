using FungleAPI.ModCompatibility.MiraSupport;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FungleAPI.GameModes.Patches
{
    [HarmonyPatch(typeof(DeadBody), nameof(DeadBody.OnClick))]
    internal static class DeadBodyPatch
    {
        public static bool Prefix(DeadBody __instance)
        {
            BaseGameMode baseGameMode = GameModeManager.GetCurrentGameMode();

            if (!baseGameMode.CanReportBodies() || MiraCompatibility.Instance != null && MiraCompatibility.Instance.GameModeBridge.IsMiraMode(baseGameMode) && MiraCompatibility.Instance.GameModeBridge.CanReport(__instance)) return false;
            return true;
        }
    }
}
