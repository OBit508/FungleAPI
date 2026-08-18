using AmongUs.GameOptions;
using FungleAPI.Utilities;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FungleAPI.GameModes.Patches
{
    [HarmonyPatch(typeof(GameOptionsManager), nameof(GameOptionsManager.SwitchGameMode))]
    internal static class GameOptionsManagerPatch
    {
        public static bool Prefix(GameOptionsManager __instance, AmongUs.GameOptions.GameModes gameMode)
        {
            __instance.currentHostOptions = __instance.normalGameHostOptions.SafeCast<IGameOptions>();
            __instance.currentSearchOptions = __instance.normalGameSearchOptions.SafeCast<IGameOptions>();
            __instance.currentGameOptions = __instance.currentNormalGameOptions.SafeCast<IGameOptions>();
            __instance.currentGameMode = gameMode;
            __instance.logger.WriteInfo(string.Format("Game options game mode switched to {0}", gameMode));
            return false;
        }
    }
}
