using FungleAPI.Event;
using FungleAPI.Event.Vanilla;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FungleAPI.Player.Patches
{
    [HarmonyPatch(typeof(PlayerPhysics))]
    internal static class PlayerPhysicsPatch
    {
        [HarmonyPrefix]
        [HarmonyPatch("RpcEnterVent")]
        public static bool RpcEnterVentPrefix(PlayerPhysics __instance, int ventId)
        {
            return !EventManager.CallEvent(new BeforeEnterVent(ventId)).Cancelled;
        }
        [HarmonyPrefix]
        [HarmonyPatch("RpcExitVent")]
        public static bool RpcExitVentPrefix(PlayerPhysics __instance, int ventId)
        {
            return !EventManager.CallEvent(new BeforeExitVent(ventId)).Cancelled;
        }
    }
}
