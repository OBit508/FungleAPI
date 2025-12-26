using FungleAPI.Role;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FungleAPI.Hud.Patches
{
    [HarmonyPatch(typeof(SabotageButton))]
    internal static class SabotageButtonPatch
    {
        [HarmonyPatch("DoClick")]
        [HarmonyPrefix]
        public static bool DoClickPrefix()
        {
            CustomRoleManager.CurrentSabotageConfig.DoClick?.Invoke();
            return false;
        }
        [HarmonyPrefix]
        [HarmonyPatch("Refresh")]
        public static bool RefreshPrefix(SabotageButton __instance)
        {
            CustomRoleManager.CurrentSabotageConfig.Refresh?.Invoke();
            return false;
        }
    }
}
