using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FungleAPI.Cosmetics.AnimatedColors;
using FungleAPI.Cosmetics.Helpers;
using FungleAPI.Translation;
using FungleAPI.Utilities.Assets;
using UnityEngine;

namespace FungleAPI.Cosmetics
{
    public class VanillaCosmetics : ModCosmetics
    {
        private List<CustomColor> colors;
        public static bool UseAnimatedColors = false;
        public override List<CustomColor> Colors
        {
            get
            {
                if (colors == null)
                {
                    colors = new List<CustomColor>();
                    for (int i = 0; i < Palette.PlayerColors.Count; i++)
                    {
                        colors.Add(new CustomColor(Palette.PlayerColors[i], Palette.ShadowColors[i], Palette.ColorNames[i]));
                    }
                    if (UseAnimatedColors)
                    {
                        colors.Add(new RainbowColor(new Translator("Rainbow").StringName));
                        colors.Add(new GalaxyColor(new Translator("Galaxy").StringName));
                        colors.Add(new FireColor(new Translator("Fire").StringName));
                    }
                }
                return colors;
            }
        }
    }
}
