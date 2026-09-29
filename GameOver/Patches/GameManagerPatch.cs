using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FungleAPI.GameOver.Patches
{
    [HarmonyPatch(typeof(GameManager))]
    internal static class GameManagerPatch
    {
        [HarmonyPatch(nameof(GameManager.DidHumansWin))]
        [HarmonyPostfix]
        public static void DidHumansWinPostfix(GameOverReason reason, ref bool __result)
        {
            __result = __result || reason == GameOverReason.HideAndSeek_CrewmatesByTimer;
        }
        [HarmonyPatch(nameof(GameManager.DidImpostorsWin))]
        [HarmonyPostfix]
        public static void DidImpostorsWinPostfix(GameOverReason reason, ref bool __result)
        {
            __result = __result || reason == GameOverReason.HideAndSeek_ImpostorsByKills;
        }
    }
}
