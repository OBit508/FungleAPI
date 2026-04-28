using AmongUs.Data;
using AsmResolver.PE.DotNet.ReadyToRun;
using FungleAPI.GameMode;
using FungleAPI.GameOver.Ends;
using FungleAPI.Role;
using FungleAPI.Role.Utilities;
using FungleAPI.Teams;
using FungleAPI.Utilities;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FungleAPI.GameOver.Patches
{
    [HarmonyPatch(typeof(LogicGameFlowNormal), "CheckEndCriteria")]
    internal static class LogicGameFlowNormalPatch
    {
        public static bool Prefix(LogicGameFlowNormal __instance)
        {
            GameModeManager.GetActiveGameMode().CheckEndCriteria(__instance.Manager);
            return false;
        }
    }
}
