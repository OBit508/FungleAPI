using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FungleAPI.Hud.Patches
{
    [HarmonyPatch(typeof(MatchInfoHudButton), nameof(MatchInfoHudButton.Update))]
    internal static class MatchInfoHudButtonPatch
    {
        public static bool Prefix(MatchInfoHudButton __instance)
        {
            if (HudManager.Instance.Chat.isActiveAndEnabled)
            {
                __instance.aspectPosition.DistanceFromEdge = MatchInfoHudButton.adjustedDistanceFromEdge;
                return false;
            }
            __instance.aspectPosition.DistanceFromEdge = MatchInfoHudButton.defaultDistanceFromEdge;
            return false;
        }
    }
}
