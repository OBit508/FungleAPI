using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FungleAPI.Roles;
using HarmonyLib;

namespace FungleAPI.Patches
{
    [HarmonyPatch(typeof(ExileController))]
    internal class ExileControllerPatchPatch
    {
        [HarmonyPatch("Begin")]
        [HarmonyPostfix]
        public static void OnExile(ExileController __instance)
        {
            if (__instance.initData.networkedPlayer != null && GameOptionsManager.Instance.currentNormalGameOptions.GetBool(AmongUs.GameOptions.BoolOptionNames.ConfirmImpostor))
            {
                string[] tx = StringNames.ExileTextSP.GetString().Split(' ', StringSplitOptions.RemoveEmptyEntries);
                __instance.completeString = __instance.initData.networkedPlayer.PlayerName + " " + tx[1] + " " + tx[2] + " " + __instance.initData.networkedPlayer.Role.NiceName;
            }
        }
    }
}
