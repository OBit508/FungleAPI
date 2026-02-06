using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FungleAPI.Cosmetics.Helpers;
using FungleAPI.Translation;
using FungleAPI.Utilities.Assets;
using UnityEngine;

namespace FungleAPI.Cosmetics
{
    public class VanillaCosmetics : ModCosmetics
    {
        private List<CustomColor> colors;
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
                }
                return colors;
            }
        }
    }
}
