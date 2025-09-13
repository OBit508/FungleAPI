using FungleAPI.Components;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FungleAPI.Patches
{
    [HarmonyPatch(typeof(GameStartManager), "Update")]
    internal static class GameStartManagerPatch
    {
        public static void Postfix(GameStartManager __instance)
        {
            __instance.StartButton.SetButtonEnableState(__instance.LastPlayerCount >= __instance.MinPlayers && LobbyWarningText.nonModdedPlayers.Count <= 0);
            if (LobbyWarningText.nonModdedPlayers.Count > 0)
            {
                __instance.StartButton.ChangeButtonText("The game cannot start because certain players do not have the FungleAPI installed.");
            }
        }
    }
}
