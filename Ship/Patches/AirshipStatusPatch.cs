using FungleAPI.GameModes;
using FungleAPI.Role;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FungleAPI.Ship.Patches
{
    [HarmonyPatch(typeof(AirshipStatus), "CalculateLightRadius")]
    internal static class AirshipStatusPatch
    {
        public static bool Prefix(NetworkedPlayerInfo player, ref float __result)
        {
            if (GameManager.Instance.IsHideAndSeek()) return true;

            __result = GameModeManager.GetCurrentGameMode().CalculateLightRadius(player, true);
            return false;
        }
    }
}
