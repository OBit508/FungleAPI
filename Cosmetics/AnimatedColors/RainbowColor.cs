using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FungleAPI.Cosmetics.Helpers;
using UnityEngine;
using static Rewired.ComponentControls.Effects.RotateAroundAxis;

namespace FungleAPI.Cosmetics.AnimatedColors
{
    public class RainbowColor : SpecialColor
    {
        public RainbowColor(StringNames colorName)
            : base(CosmeticManager.GetValidProxyColor(), CosmeticManager.GetValidProxyColor(), colorName) { }
        public override void UpdateMaterial(Material material)
        {
            material.SetColor(PlayerMaterial.BackColor, AnimatedColorsUtils.Rainbow);
            material.SetColor(PlayerMaterial.BodyColor, AnimatedColorsUtils.RainbowShadow);
            material.SetColor(PlayerMaterial.VisorColor, Palette.VisorColor);
        }
        public override Color BaseColor => AnimatedColorsUtils.Rainbow;
    }
}
