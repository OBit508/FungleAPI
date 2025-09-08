using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FungleAPI.Role.Teams;
using FungleAPI.Role;
using FungleAPI.Utilities;
using HarmonyLib;

namespace FungleAPI.Patches
{
    [HarmonyPatch(typeof(ExileController))]
    internal static class ExileControllerPatchPatch
    {
        [HarmonyPatch("Begin")]
        [HarmonyPostfix]
        public static void BeginPostfix(ExileController __instance)
        {
            if (__instance.initData.networkedPlayer != null && GameOptionsManager.Instance.currentNormalGameOptions.GetBool(AmongUs.GameOptions.BoolOptionNames.ConfirmImpostor))
            {
                string[] tx = StringNames.ExileTextSP.GetString().Split(' ', StringSplitOptions.RemoveEmptyEntries);
                __instance.completeString = __instance.initData.networkedPlayer.PlayerName + " " + tx[1] + " " + tx[2] + " " + __instance.initData.networkedPlayer.Role.NiceName;
            }
        }
        [HarmonyPatch("ReEnableGameplay")]
        [HarmonyPostfix]
        public static void ReEnableGameplayPostfix()
        {
            foreach (CustomAbilityButton button in CustomAbilityButton.Buttons.Values)
            {
                button.Reset();
            }
        }
    }
}
