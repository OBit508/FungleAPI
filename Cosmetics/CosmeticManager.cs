using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FungleAPI.Cosmetics.Helpers;
using Innersloth.Assets;
using UnityEngine;

namespace FungleAPI.Cosmetics
{
    public static class CosmeticManager
    {
        internal const float r = 0.618033988749895f;
        internal static float hue = UnityEngine.Random.value;
        public static List<CustomColor> AllColors = new List<CustomColor>();
        public static List<CustomColor> SpecialColors = new List<CustomColor>();
        public static bool IsSpecialColor(int colorId, out SpecialColor specialColor)
        {
            specialColor = (SpecialColor)SpecialColors.FirstOrDefault(c => c.ColorId == colorId);
            return specialColor != null;
        }
        public static void Add(ModCosmetics modCosmetics)
        {
            if (modCosmetics.Colors != null && modCosmetics.Colors.Count > 0)
            {
                AllColors = AllColors.Concat(modCosmetics.Colors).ToList();
                SpecialColors = SpecialColors.Concat(modCosmetics.Colors.FindAll(c => c is SpecialColor)).ToList();
            }
        }
        public static Color GetBaseColor(int colorId)
        {
            CustomColor color = AllColors.FirstOrDefault(c => c.ColorId == colorId);
            if (color is SpecialColor specialColor)
            {
                return specialColor.BaseColor;
            }
            return color == null ? Color.black : color.Color;
        }
        public static Color GetValidProxyColor()
        {
            hue = (hue + r) % 1f;
            return Color.HSVToRGB(hue, 0.85f, 0.95f);
        }
        internal static void SetPaletta()
        {
            List<Color32> PlayerColors = new List<Color32>();
            List<Color32> ShadowColors = new List<Color32>();
            List<StringNames> ColorsNames = new List<StringNames>();
            for (int i = 0; i < AllColors.Count; i++)
            {
                CustomColor color = AllColors[i];
                PlayerColors.Add(color.Color);
                ShadowColors.Add(color.BackColor);
                ColorsNames.Add(color.ColorName);
                color.ColorId = i;
            }
            Palette.PlayerColors = PlayerColors.ToArray();
            Palette.ShadowColors = ShadowColors.ToArray();
            Palette.ColorNames = ColorsNames.ToArray();
        }
    }
}
