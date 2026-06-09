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
            SpecialColorBehaviour specialColorBehaviour;
            if (CosmeticManager.IsSpecialColor(colorId, out SpecialColor color))
            {
                specialColorBehaviour = __instance.Renderer.gameObject.GetOrAddComponent<SpecialColorBehaviour>();
                specialColorBehaviour.Color = color;
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
