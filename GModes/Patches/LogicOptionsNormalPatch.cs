using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FungleAPI.GModes.Patches
{
    [HarmonyPatch(typeof(LogicOptionsNormal))]
    internal static class LogicOptionsNormalPatch
    {
        [HarmonyPatch(nameof(LogicOptionsNormal.GetDiscussionTime))]
        [HarmonyPrefix]
        public static bool GetDiscussionTime(ref int __result)
        {
            __result = GameModeManager.GetCurrentGameMode().GetDiscussionTime();
            return false;
        }
        [HarmonyPatch(nameof(LogicOptionsNormal.GetVotingTime))]
        [HarmonyPrefix]
        public static bool GetVotingTime(ref int __result)
        {
            __result = GameModeManager.GetCurrentGameMode().GetVotingTime();
            return false;
        }
    }
}
