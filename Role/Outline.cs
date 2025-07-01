using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FungleAPI.Patches;
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
                if (active && __instance.Data.Role as ICustomRole != null)
                {
                    __instance.cosmetics.currentBodySprite.BodySprite.material.SetFloat("_Outline", 1f);
                    __instance.cosmetics.currentBodySprite.BodySprite.material.SetColor("_OutlineColor", (__instance.Data.Role as ICustomRole).RoleColor);
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
                ICustomRole role = PlayerControl.LocalPlayer.Data.Role as ICustomRole;
                if (role != null)
                {
                    if (on)
                    {
                        __instance.myRend.material.SetFloat("_Outline", 1f);
                        __instance.myRend.material.SetColor("_OutlineColor", role.RoleColor);
                    }
                    else
                    {
                        __instance.myRend.material.SetFloat("_Outline", 0f);
                    }
                    if (mainTarget)
                    {
                        float num = Mathf.Clamp01(role.RoleColor.r * 0.5f);
                        float num2 = Mathf.Clamp01(role.RoleColor.g * 0.5f);
                        float num3 = Mathf.Clamp01(role.RoleColor.b * 0.5f);
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
