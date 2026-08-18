using AmongUs.GameOptions;
using FungleAPI.Api;
using FungleAPI.GameModes;
using FungleAPI.Utilities;
using HarmonyLib;
using Il2CppInterop.Runtime;
using Steamworks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FungleAPI.GameModes.Patches
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
        [HarmonyPatch(nameof(NormalGameManager.GetBodyType))]
        [HarmonyPrefix]
        public static bool GetBodyTypePrefix(ref PlayerBodyTypes __result, PlayerControl player)
        {
            __result = GameModeManager.GetCurrentGameMode().GetBodyType(player);
            return false;
        }
    }
}
