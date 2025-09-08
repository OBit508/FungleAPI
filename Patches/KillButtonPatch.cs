using FungleAPI.Role;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace FungleAPI.Patches
{
    [HarmonyPatch(typeof(KillButton), "SetTarget")]
    internal static class KillButtonPatch
    {
        public static bool Prefix(KillButton __instance, PlayerControl target)
        {
            if (!PlayerControl.LocalPlayer || PlayerControl.LocalPlayer.Data == null || !PlayerControl.LocalPlayer.Data.Role)
            {
                return false;
            }
            ICustomRole customRole = PlayerControl.LocalPlayer.Data.Role.CustomRole();
            if (customRole == null)
            {
                return true;
            }
            if (__instance.currentTarget && __instance.currentTarget != target)
            {
                __instance.currentTarget.cosmetics.SetOutline(false, new Il2CppSystem.Nullable<Color>(Color.clear));
            }
            __instance.currentTarget = target;
            if (__instance.currentTarget)
            {
                __instance.currentTarget.cosmetics.SetOutline(true, new Il2CppSystem.Nullable<Color>(customRole.Configuration.OutlineColor));
                __instance.SetEnabled();
                return false;
            }
            __instance.SetDisabled();
            return false;
        }
    }
}
