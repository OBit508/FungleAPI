using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FungleAPI.Cosmetics.Helpers;
using UnityEngine;

namespace FungleAPI.Cosmetics.AnimatedColors
{
    public class GalaxyColor : SpecialColor
    {
        public GalaxyColor(StringNames colorName)
           : base(CosmeticManager.GetValidProxyColor(), CosmeticManager.GetValidProxyColor(), colorName) { }
        public override void UpdateMaterial(Material material)
        {
            material.SetColor(PlayerMaterial.BackColor, AnimatedColorsUtils.Galaxy);
            material.SetColor(PlayerMaterial.BodyColor, AnimatedColorsUtils.GalaxyShadow);
            material.SetColor(PlayerMaterial.VisorColor, Palette.VisorColor);
        }
        public override Color BaseColor => AnimatedColorsUtils.Galaxy;
    }
}
