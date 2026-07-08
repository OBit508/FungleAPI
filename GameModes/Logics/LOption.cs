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
    [HarmonyPatch(typeof(LogicOptions))]
    internal static class LOptions
    {
        [HarmonyPatch(nameof(LogicOptions.GetGhostsDoTasks))]
        [HarmonyPrefix]
        public static bool GetGhostsDoTasks(ref bool __result)
        {
            __result = GameModeManager.GetCurrentGameMode().GetGhostsDoTasks();
            return false;
        }
        [HarmonyPatch(nameof(LogicOptions.GetKillCooldown))]
        [HarmonyPrefix]
        public static bool GetKillCooldown(ref float __result)
        {
            __result = GameModeManager.GetCurrentGameMode().GetKillCooldown();
            return false;
        }
        [HarmonyPatch(nameof(LogicOptions.GetKillDistance))]
        [HarmonyPrefix]
        public static bool GetKillDistance(ref float __result)
        {
            __result = GameModeManager.GetCurrentGameMode().GetKillDistance();
            return false;
        }
        [HarmonyPatch(nameof(LogicOptions.GetPlayerSpeedMod))]
        [HarmonyPrefix]
        public static bool GetPlayerSpeedMod(PlayerControl pc, ref float __result)
        {
            __result = GameModeManager.GetCurrentGameMode().GetPlayerSpeedMod(pc);
            return false;
        }
        [HarmonyPatch(nameof(LogicOptions.GetEngineerCooldown))]
        [HarmonyPrefix]
        public static bool GetEngineerCooldown(ref float __result)
        {
            __result = GameModeManager.GetCurrentGameMode().GetEngineerCooldown();
            return false;
        }
        [HarmonyPatch(nameof(LogicOptions.GetEngineerInVentTime))]
        [HarmonyPrefix]
        public static bool GetEngineerInVentTime(ref float __result)
        {
            __result = GameModeManager.GetCurrentGameMode().GetEngineerInVentTime();
            return false;
        }
    }
}
