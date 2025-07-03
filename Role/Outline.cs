using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FungleAPI.Patches;
using FungleAPI.Role;
using HarmonyLib;
using UnityEngine;

namespace FungleAPI.Roles
{
    internal class Outline
    {
        [HarmonyPatch(typeof(PlayerControl), "ToggleHighlight")]
        public static class PlayerOutline
        {
            public static bool Prefix(PlayerControl __instance, [HarmonyArgument(0)] bool active)
            {
                if (__instance.Data.Role.CustomRole() != null)
                {
                    __instance.cosmetics.SetOutline(active, new Il2CppSystem.Nullable<Color>(__instance.Data.Role.CustomRole().CachedConfiguration.OutlineColor));
                    return false;
                }
                return true;
            }
        }
        [HarmonyPatch(typeof(Vent), "SetOutline")]
        public static class VentOutline
        {
            public static bool Prefix(Vent __instance, [HarmonyArgument(0)] bool on, [HarmonyArgument(1)] bool mainTarget)
            {
                ICustomRole role = PlayerControl.LocalPlayer.Data.Role.CustomRole();
                if (role != null)
                {
                    if (on)
                    {
                        __instance.myRend.material.SetFloat("_Outline", 1f);
                        __instance.myRend.material.SetColor("_OutlineColor", role.CachedConfiguration.OutlineColor);
                    }
                    else
                    {
                        __instance.myRend.material.SetFloat("_Outline", 0f);
                    }
                    if (mainTarget)
                    {
                        float num = Mathf.Clamp01(role.CachedConfiguration.OutlineColor.r * 0.5f);
                        float num2 = Mathf.Clamp01(role.CachedConfiguration.OutlineColor.g * 0.5f);
                        float num3 = Mathf.Clamp01(role.CachedConfiguration.OutlineColor.b * 0.5f);
                        __instance.myRend.material.SetColor("_AddColor", new Color(num, num2, num3, 1f));
                    }
                    else
                    {
                        __instance.myRend.material.SetColor("_AddColor", new Color(0f, 0f, 0f, 0f));
                    }
                    return false;
                }
                return true;
            }
        }
    }
}
