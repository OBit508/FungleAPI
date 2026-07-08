using AmongUs.GameOptions;
using FungleAPI.Attributes;
using FungleAPI.Utilities;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace FungleAPI.GameModes.Logics
{
    [HarmonyPatch(typeof(LogicOptionsNormal))]
    internal static class LOptionsN
    {
        [HarmonyPatch(nameof(LogicOptionsNormal.GetConfirmImpostor))]
        [HarmonyPrefix]
        public static bool GetConfirmImpostor(ref bool __result)
        {
            __result = GameModeManager.GetCurrentGameMode().GetConfirmImpostor();
            return false;
        }
        [HarmonyPatch(nameof(LogicOptionsNormal.GetEmergencyCooldown))]
        [HarmonyPrefix]
        public static bool GetEmergencyCooldown(ref int __result)
        {
            __result = GameModeManager.GetCurrentGameMode().GetEmergencyCooldown();
            return false;
        }
        [HarmonyPatch(nameof(LogicOptionsNormal.GetNumEmergencyMeetings))]
        [HarmonyPrefix]
        public static bool GetNumEmergencyMeetings(ref int __result)
        {
            __result = GameModeManager.GetCurrentGameMode().GetNumEmergencyMeetings();
            return false;
        }
        [HarmonyPatch(nameof(LogicOptionsNormal.GetVisualTasks))]
        [HarmonyPrefix]
        public static bool GetVisualTasks(ref bool __result)
        {
            __result = GameModeManager.GetCurrentGameMode().GetVisualTasks();
            return false;
        }
        [HarmonyPatch(nameof(LogicOptionsNormal.GetAnonymousVotes))]
        [HarmonyPrefix]
        public static bool GetAnonymousVotes(ref bool __result)
        {
            __result = GameModeManager.GetCurrentGameMode().GetAnonymousVotes();
            return false;
        }
        [HarmonyPatch(nameof(LogicOptionsNormal.GetTaskBarMode))]
        [HarmonyPrefix]
        public static bool GetTaskBarMode(ref TaskBarMode __result)
        {
            __result = GameModeManager.GetCurrentGameMode().GetTaskBarMode();
            return false;
        }
    }
}
