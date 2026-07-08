using FungleAPI.Attributes;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FungleAPI.GameModes.Logics
{
    [HarmonyPatch(typeof(LogicGameFlowNormal))]
    internal static class LGameFlow
    {
        [HarmonyPatch(nameof(LogicGameFlowNormal.CheckEndCriteria))]
        [HarmonyPrefix]
        public static bool CheckEndCriteria()
        {
            GameModeManager.GetCurrentGameMode().CheckEndCriteria();
            return false;
        }
    }
}
