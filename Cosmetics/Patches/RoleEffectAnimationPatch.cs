using FungleAPI.Components;
using FungleAPI.Cosmetics.Colors;
using FungleAPI.Extensions;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FungleAPI.Cosmetics.Patches
{
    [HarmonyPatch(typeof(RoleEffectAnimation), nameof(RoleEffectAnimation.SetMaterialColor))]
    internal static class RoleEffectAnimationPatch
    {
        public static void Postfix(RoleEffectAnimation __instance, int colorId)
        {
            CustomColor customColor = CosmeticManager.AllColors.FirstOrDefault(c => c.ColorId == colorId);

            if (customColor == null) return;

            __instance.Renderer.material.SetColor("_VisorColor", customColor.VisorColor);

            SpecialColorBehaviour specialColorBehaviour;
            if (customColor is SpecialColor specialColor)
            {
                specialColorBehaviour = __instance.Renderer.gameObject.GetOrAddComponent<SpecialColorBehaviour>();
                specialColorBehaviour.Color = specialColor;
                specialColorBehaviour.Mat = __instance.Renderer.material;
            }
            else
            {
                specialColorBehaviour = __instance.Renderer.GetComponent<SpecialColorBehaviour>();
                if (specialColorBehaviour != null)
                {
                    specialColorBehaviour.Color = null;
                    specialColorBehaviour.Mat = null;
                }
            }
        }
    }
}
