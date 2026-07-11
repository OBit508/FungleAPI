using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FungleAPI.GameModes.Patches
{
    [HarmonyPatch(typeof(MapConsole), nameof(MapConsole.CanUse))]
    internal static class MapConsolePatch
    {
        public static bool Prefix(MapConsole __instance, NetworkedPlayerInfo pc, ref bool canUse, ref bool couldUse, ref float __result)
        {
            if (GameManager.Instance.IsHideAndSeek()) return true;

            __result = GameModeManager.GetCurrentGameMode().CanUseMapConsole(__instance, pc, out canUse, out couldUse);
            return false;
        }
    }
}
