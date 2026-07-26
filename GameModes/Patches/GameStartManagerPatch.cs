using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FungleAPI.GameModes.Patches
{
    [HarmonyPatch(typeof(GameStartManager), nameof(GameStartManager.Start))]
    internal static class GameStartManagerPatch
    {
        public static void Prefix(GameStartManager __instance)
        {
            __instance.MinPlayers = GameModeManager.GetCurrentGameMode().RequiredPlayerToStart();
        }
    }
}
