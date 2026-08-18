using FungleAPI.Role.Utilities;
using FungleAPI.Teams;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FungleAPI.GameModes.Patches
{
    [HarmonyPatch(typeof(Minigame))]
    internal static class MinigamePatch
    {
        [HarmonyPatch(typeof(Minigame), nameof(Minigame.CoAnimateOpen))]
        [HarmonyPrefix]
        public static void CoAnimateOpenPrefix()
        {
            if (PlayerControl.LocalPlayer.Data.Role.GetTeam() != ModdedTeamManager.Crewmates) return;

            GameModeManager.GetCurrentGameMode().OnMinigameOpen();
        }
        [HarmonyPatch(typeof(Minigame), nameof(Minigame.Close), new Type[0])]
        [HarmonyPrefix]
        public static void ClosePrefix()
        {
            if (PlayerControl.LocalPlayer.Data.Role.GetTeam() != ModdedTeamManager.Crewmates) return;

            GameModeManager.GetCurrentGameMode().OnMinigameClose();
        }
    }
}
