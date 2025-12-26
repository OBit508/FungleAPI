using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FungleAPI.Role;
using HarmonyLib;

namespace FungleAPI.Hud.Patches
{
    [HarmonyPatch(typeof(VentButton))]
    internal static class VentButtonPatch
    {
        [HarmonyPatch("SetTarget")]
        [HarmonyPrefix]
        public static bool SetTargetPrefix(VentButton __instance, [HarmonyArgument(0)] Vent target)
        {
            CustomRoleManager.CurrentVentConfig.SetTarget?.Invoke(target);
            return false;
        }
        [HarmonyPatch("DoClick")]
        [HarmonyPrefix]
        public static bool DoClickPrefix(VentButton __instance)
        {
            CustomRoleManager.CurrentVentConfig.DoClick?.Invoke();
            return false;
        }
    }
}
