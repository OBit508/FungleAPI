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
                if (active)
                {
                    __instance.cosmetics.currentBodySprite.BodySprite.material.SetFloat("_Outline", 1f);
                    __instance.cosmetics.currentBodySprite.BodySprite.material.SetColor("_OutlineColor", PlayerControl.LocalPlayer.Data.Role.TeamColor);
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
                if (on)
                {
                    __instance.myRend.material.SetFloat("_Outline", 1f);
                    __instance.myRend.material.SetColor("_OutlineColor", PlayerControl.LocalPlayer.Data.Role.TeamColor);
                }
                else
                {
                    __instance.myRend.material.SetFloat("_Outline", 0f);
                }
                if (mainTarget)
                {
                    float num = Mathf.Clamp01(PlayerControl.LocalPlayer.Data.Role.TeamColor.r * 0.5f);
                    float num2 = Mathf.Clamp01(PlayerControl.LocalPlayer.Data.Role.TeamColor.g * 0.5f);
                    float num3 = Mathf.Clamp01(PlayerControl.LocalPlayer.Data.Role.TeamColor.b * 0.5f);
                    __instance.myRend.material.SetColor("_AddColor", new Color(num, num2, num3, 1f));
                }
                else
                {
                    __instance.myRend.material.SetColor("_AddColor", new Color(0f, 0f, 0f, 0f));
                }
                return false;
            }
        }
    }
}
