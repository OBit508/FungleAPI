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
        public override Color BaseColor => new HSBColor(Ping(0f, 1f, 0.25f), 0.9f, 1f).ToColor();
        private static float Ping(float min, float max, float speed)
        {
            return min + Mathf.PingPong(Time.time * speed, max - min);
        }
        public RainbowColor(StringNames colorName)
            : base(CosmeticManager.GetValidProxyColor(), CosmeticManager.GetValidProxyColor(), colorName) { }
        public override void UpdateMaterial(Material material)
        {
            material.SetColor(PlayerMaterial.BackColor, BaseColor);
            material.SetColor(PlayerMaterial.BodyColor, Utilities.Helpers.Dark(BaseColor, 0.35f));
            material.SetColor(PlayerMaterial.VisorColor, Palette.VisorColor);
        }
    }
}
