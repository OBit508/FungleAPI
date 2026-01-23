using FungleAPI.Role;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FungleAPI.Patches
{
    [HarmonyPatch(typeof(AirshipStatus), "CalculateLightRadius")]
    internal static class AirshipStatusPatch
    {
        public static bool Prefix([HarmonyArgument(0)] NetworkedPlayerInfo player, ref float __result)
        {
            __result = RoleConfigManager.LightConfig.CalculateLightRadius(player, true);
            return false;
        }
    }
}
