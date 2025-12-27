using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FungleAPI.Cosmetics.Helpers;
using UnityEngine;

namespace FungleAPI.Cosmetics.AnimatedColors
{
    public class FireColor : SpecialColor
    {
        private static float fireNextChange;
        private static Color fireCurrent;
        private static Color fireTarget;
        public override Color BaseColor
        {
            get
            {
                if (Time.time >= fireNextChange)
                {
                    fireNextChange = Time.time + UnityEngine.Random.Range(0.25f, 0.5f);
                    fireTarget = new HSBColor(UnityEngine.Random.Range(0.03f, 0.12f), UnityEngine.Random.Range(0.85f, 1f), UnityEngine.Random.Range(0.5f, 0.9f)).ToColor();
                }
                fireCurrent = Color.Lerp(fireCurrent == default ? fireTarget : fireCurrent, fireTarget, Time.deltaTime * 2.5f);
                return fireCurrent;
            }
        }
        public FireColor(StringNames colorName)
           : base(CosmeticManager.GetValidProxyColor(), CosmeticManager.GetValidProxyColor(), colorName) { }
        public override void UpdateMaterial(Material material)
        {
            material.SetColor(PlayerMaterial.BackColor, BaseColor);
            material.SetColor(PlayerMaterial.BodyColor, Utilities.Helpers.Dark(BaseColor, 0.5f));
            material.SetColor(PlayerMaterial.VisorColor, Palette.VisorColor);
        }
    }
}
