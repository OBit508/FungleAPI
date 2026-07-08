using FungleAPI.Attributes;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FungleAPI.GameModes.Logics
{
    [HarmonyPatch(typeof(LogicUsablesBasic))]
    internal static class LUsasbles
    {
        [HarmonyPatch(nameof(LogicUsablesBasic.CanUse))]
        [HarmonyPrefix]
        public static bool CanUse(IUsable usable, PlayerControl player, ref bool __result)
        {
            __result = GameModeManager.GetCurrentGameMode().CanUse(usable, player);
            return false;
        }
    }
}
