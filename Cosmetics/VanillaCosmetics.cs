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
        private List<CustomColor> __colors;
        public override List<CustomColor> Colors
        {
            get
            {
                if (__colors == null)
                {
                    __colors = new List<CustomColor>();
                    for (int i = 0; i < Palette.PlayerColors.Count; i++)
                    {
                        __colors.Add(new CustomColor(Palette.PlayerColors[i], Palette.ShadowColors[i], Palette.ColorNames[i]));
                    }
                    __colors.Add(new RainbowColor(new Translator("Rainbow").StringName));
                    __colors.Add(new GalaxyColor(new Translator("Galaxy").StringName));
                    __colors.Add(new FireColor(new Translator("Fire").StringName));
                }
                return __colors;
            }
        }
    }
}
