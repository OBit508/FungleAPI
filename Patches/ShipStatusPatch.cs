using FungleAPI.Components;
using FungleAPI.Event;
using FungleAPI.Event.Types;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FungleAPI.Patches
{
    [HarmonyPatch(typeof(ShipStatus))]
    internal static class ShipStatusPatch
    {
        [HarmonyPatch("Awake")]
        [HarmonyPostfix]
        public static void AwakePostfix()
        {
            VentHelper.ShipVents.Clear();
        }
        [HarmonyPatch("CloseDoorsOfType")]
        [HarmonyPostfix]
        public static void CloseDoorsOfTypePostfix([HarmonyArgument(0)] SystemTypes room)
        {
            EventManager.CallEvent(new OnCloseDoor() { Room = room });
        }
        [HarmonyPatch("UpdateSystem", new Type[] { typeof(SystemTypes), typeof(PlayerControl), typeof(byte) })]
        [HarmonyPostfix]
        public static void UpdateSystemPostfix([HarmonyArgument(0)] SystemTypes systemType, [HarmonyArgument(1)] PlayerControl player, [HarmonyArgument(2)] byte amount)
        {
            EventManager.CallEvent(new OnUpdateSystem() { Amount = amount, Player = player, SystemType = systemType });
        }
    }
}
