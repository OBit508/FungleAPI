using AmongUs.GameOptions;
using FungleAPI.GModes;
using FungleAPI.Player;
using FungleAPI.Role.Utilities;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FungleAPI.Patches
{
    [HarmonyPatch(typeof(NormalGameManager))]
    internal static class NormalGameManagerPatch
    {
        [HarmonyPatch("GetDeadBody")]
        [HarmonyPrefix]
        public static bool GetDeadBodyPrefix(GameManager __instance, RoleBehaviour impostorRole, ref DeadBody __result)
        {
            __result = GameModeManager.GetCurrentGameMode().GetDeadBody(__instance, impostorRole);
            return false;
        }
        [HarmonyPatch("GetMapOptions")]
        [HarmonyPrefix]
        public static bool GetMapOptionsPrefix(ref MapOptions __result)
        {
            __result = GameModeManager.GetCurrentGameMode().GetMapOptions();
            return false;
        }
    }
}
