using FungleAPI.Assets;
using FungleAPI.Cosmetics.Colors;
using FungleAPI.Extensions;
using FungleAPI.Translation;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace FungleAPI.Cosmetics
{
    public class VanillaCosmetics : ModCosmetics
    {
        public override void Initialize()
        {
            List<CustomColor> colors = new List<CustomColor>();
            for (int i = 0; i < Palette.PlayerColors.Count; i++)
            {
                colors.Add(new CustomColor(Palette.PlayerColors[i], Palette.ShadowColors[i], Palette.ColorNames[i]));
            }
            Colors = colors.ToArray();
        }
    }
}
