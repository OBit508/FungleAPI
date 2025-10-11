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
            if (PlayerControl.LocalPlayer.Data.Role.CanSabotage() && !PlayerControl.LocalPlayer.inVent && GameManager.Instance.SabotagesEnabled())
            {
                HudManager.Instance.ToggleMapVisible(new MapOptions
                {
                    Mode = MapOptions.Modes.Sabotage
                });
            }
            return false;
        }
        [HarmonyPrefix]
        [HarmonyPatch("Refresh")]
        public static bool RefreshPrefix(SabotageButton __instance)
        {
            PlayerControl player = PlayerControl.LocalPlayer;
            if (GameManager.Instance == null || player == null)
            {
                __instance.ToggleVisible(false);
                __instance.SetDisabled();
                return false;
            }
            if (player.inVent || !GameManager.Instance.SabotagesEnabled() || player.petting)
            {
                __instance.ToggleVisible(player.Data.Role.CanSabotage() && GameManager.Instance.SabotagesEnabled());
                __instance.SetDisabled();
                return false;
            }
            __instance.SetEnabled();
            return false;
        }
    }
}
