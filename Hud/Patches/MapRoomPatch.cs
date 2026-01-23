using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FungleAPI.Role;
using HarmonyLib;

namespace FungleAPI.Hud.Patches
{
    [HarmonyPatch(typeof(MapRoom))]
    internal static class MapRoomPatch
    {
        [HarmonyPatch("SabotageComms")]
        [HarmonyPrefix]
        public static void SabotageCommsPrefix(MapRoom __instance)
        {
            if (__instance.Parent.CanUseSabotage)
            {
                RoleConfigManager.SabotageConfig.ResetTimer?.Invoke(false);
            }
        }
        [HarmonyPatch("SabotageDoors")]
        [HarmonyPrefix]
        public static void SabotageDoorsPrefix(MapRoom __instance)
        {
            if (__instance.Parent.CanUseDoors)
            {
                RoleConfigManager.SabotageConfig.ResetTimer?.Invoke(true);
            }
        }
        [HarmonyPatch("SabotageHeli")]
        [HarmonyPrefix]
        public static void SabotageHeliPrefix(MapRoom __instance)
        {
            if (__instance.Parent.CanUseSabotage)
            {
                RoleConfigManager.SabotageConfig.ResetTimer?.Invoke(false);
            }
        }
        [HarmonyPatch("SabotageLights")]
        [HarmonyPrefix]
        public static void SabotageLightsPrefix(MapRoom __instance)
        {
            if (__instance.Parent.CanUseSabotage)
            {
                RoleConfigManager.SabotageConfig.ResetTimer?.Invoke(false);
            }
        }
        [HarmonyPatch("SabotageMushroomMixup")]
        [HarmonyPrefix]
        public static void SabotageMushroomMixupPrefix(MapRoom __instance)
        {
            if (__instance.Parent.CanUseSabotage)
            {
                RoleConfigManager.SabotageConfig.ResetTimer?.Invoke(false);
            }
        }
        [HarmonyPatch("SabotageOxygen")]
        [HarmonyPrefix]
        public static void SabotageOxygenPrefix(MapRoom __instance)
        {
            if (__instance.Parent.CanUseSabotage)
            {
                RoleConfigManager.SabotageConfig.ResetTimer?.Invoke(false);
            }
        }
        [HarmonyPatch("SabotageReactor")]
        [HarmonyPrefix]
        public static void SabotageReactorPrefix(MapRoom __instance)
        {
            if (__instance.Parent.CanUseSabotage)
            {
                RoleConfigManager.SabotageConfig.ResetTimer?.Invoke(false);
            }
        }
        [HarmonyPatch("SabotageSeismic")]
        [HarmonyPrefix]
        public static void SabotageSeismicPrefix(MapRoom __instance)
        {
            if (__instance.Parent.CanUseSabotage)
            {
                RoleConfigManager.SabotageConfig.ResetTimer?.Invoke(false);
            }
        }
    }
}
