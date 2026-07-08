using FungleAPI.Attributes;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FungleAPI.GameModes.Logics
{
    [HarmonyPatch(typeof(LogicRoleSelectionNormal))]
    internal static class LRoleSelection
    {
        [HarmonyPatch(nameof(LogicRoleSelectionNormal.OnPlayerDeath))]
        [HarmonyPrefix]
        public static bool OnPlayerDeath(PlayerControl player, bool assignGhostRole)
        {
            GameModeManager.GetCurrentGameMode().OnPlayerDeath(player, assignGhostRole);
            return false;
        }
    }
}
