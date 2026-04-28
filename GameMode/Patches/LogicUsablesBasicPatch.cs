using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FungleAPI.GameMode.Patches
{
    [HarmonyPatch(typeof(LogicUsablesBasic), "CanUse")]
    internal static class LogicUsablesBasicPatch
    {
        public static bool Prefix(IUsable usable, PlayerControl player, ref bool __result)
        {
            __result = GameModeManager.GetActiveGameMode().CanUse(usable, player);
            return false;
        }
    }
}
