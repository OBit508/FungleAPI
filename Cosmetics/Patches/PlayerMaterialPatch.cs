using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FungleAPI.Components;
using FungleAPI.Cosmetics.Helpers;
using FungleAPI.Utilities;
using HarmonyLib;
using UnityEngine;

namespace FungleAPI.Cosmetics.Patches
{
    [HarmonyPatch(typeof(PlayerMaterial))]
    internal static class PlayerMaterialPatch
    {
        [HarmonyPatch("SetColors", new Type[] { typeof(int), typeof(Renderer) })]
        [HarmonyPostfix]
        public static void SetColorsPostfix([HarmonyArgument(0)] int colorId, [HarmonyArgument(1)] Renderer rend)
        {
            SpecialColorBehaviour specialColorBehaviour;
            if (CosmeticManager.IsSpecialColor(colorId, out SpecialColor color))
            {
                specialColorBehaviour = rend.gameObject.GetOrAddComponent<SpecialColorBehaviour>();
                specialColorBehaviour.Color = color;
                specialColorBehaviour.Mat = rend.material;
            }
            else
            {
                specialColorBehaviour = rend.GetComponent<SpecialColorBehaviour>();
                if (specialColorBehaviour != null)
                {
                    specialColorBehaviour.Color = null;
                    specialColorBehaviour.Mat = null;
                }
            }
        }
    }
}
