using FungleAPI.Components;
using FungleAPI.Event;
using FungleAPI.Event.Vanilla;
using FungleAPI.GameMode;
using FungleAPI.Role;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FungleAPI.Ship.Patches
{
    [HarmonyPatch(typeof(ShipStatus))]
    internal static class ShipStatusPatch
    {
        [HarmonyPatch("Awake")]
        [HarmonyPostfix]
        public static void AwakePostfix()
        {
            Vent[] vents = VentHelper.ShipVents.Keys.ToArray();
            for (int i = 0; i < vents.Count(); i++)
            {
                if (vents[i] == null)
                {
                    VentHelper.ShipVents.Remove(vents[i]);
                }
            }
        }
        [HarmonyPatch("Begin")]
        [HarmonyPrefix]
        public static bool BeginPrefix(ShipStatus __instance)
        {
            if (GameManager.Instance.LogicRoleSelection is LogicRoleSelectionNormal)
            {
                GameModeManager.GetActiveGameMode().AssignTasks(__instance);
                PlayerControl.LocalPlayer.cosmetics.SetAsLocalPlayer();
                return false;
            }
            return true;
        }
        [HarmonyPatch("CloseDoorsOfType")]
        [HarmonyPostfix]
        public static void CloseDoorsOfTypePostfix(SystemTypes room)
        {
            EventManager.CallEvent(new CloseDoorEvent(room));
        }
        [HarmonyPatch("UpdateSystem", new Type[] { typeof(SystemTypes), typeof(PlayerControl), typeof(byte) })]
        [HarmonyPostfix]
        public static void UpdateSystemPostfix(SystemTypes systemType, PlayerControl player, byte amount)
        {
            EventManager.CallEvent(new UpdateSystemEvent(systemType, player, amount));
        }
        [HarmonyPatch("CalculateLightRadius")]
        [HarmonyPrefix]
        public static bool CalculateLightRadiusPrefix(NetworkedPlayerInfo player, ref float __result)
        {
            __result = RoleConfigManager.LightConfig.CalculateLightRadius(player, false);
            return false;
        }
    }
}
