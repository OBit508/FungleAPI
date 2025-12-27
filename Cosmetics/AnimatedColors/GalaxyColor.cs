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
        public override Color BaseColor
        {
            get
            {
                float cycle = Mathf.Repeat(Time.time, 3f);
                float flashStrength = 0f;
                if (cycle < 0.1f)
                {
                    float x = cycle / 0.1f;
                    flashStrength = Mathf.SmoothStep(0f, 0.35f, x);
                }
                else if (cycle < 0.2f)
                {
                    float x = (cycle - 0.1f) / 0.1f;
                    flashStrength = Mathf.SmoothStep(0.35f, 0f, x);
                }
                return Color.Lerp(Color.Lerp(new Color(0.32f, 0.12f, 0.55f), new Color(0.08f, 0.18f, 0.42f), Mathf.PingPong(Time.time * 0.15f, 1f)), Color.white, flashStrength);
            }
        }
        public GalaxyColor(StringNames colorName)
           : base(CosmeticManager.GetValidProxyColor(), CosmeticManager.GetValidProxyColor(), colorName) { }
        public override void UpdateMaterial(Material material)
        {
            material.SetColor(PlayerMaterial.BackColor, BaseColor);
            material.SetColor(PlayerMaterial.BodyColor, Utilities.Helpers.Dark(BaseColor, 0.45f));
            material.SetColor(PlayerMaterial.VisorColor, Palette.VisorColor);
        }
    }
}
