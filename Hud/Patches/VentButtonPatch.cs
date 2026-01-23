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
            RoleConfigManager.VentConfig.SetTarget?.Invoke(target);
            return false;
        }
        [HarmonyPatch("DoClick")]
        [HarmonyPrefix]
        public static bool DoClickPrefix(VentButton __instance)
        {
            RoleConfigManager.VentConfig.DoClick?.Invoke();
            return false;
        }
    }
}
