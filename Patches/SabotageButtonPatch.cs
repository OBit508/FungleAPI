using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FungleAPI.Roles;
using HarmonyLib;

namespace FungleAPI.Patches
{
    [HarmonyPatch(typeof(SabotageButton), "DoClick")]
    internal static class SabotageButtonPatch
    {
        public static bool Prefix()
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
    }
}
