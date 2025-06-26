using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace FungleAPI.Cosmetic
{
    public class CustomColor
    {
        public static List<CustomColor> AllCustomColors = new List<CustomColor>();
        public CustomColor(Color32 color, Color32 shadowColor, string colorName)
        {
            Color = color;
            ShadowColor = shadowColor;
            ColorName = colorName;
            this.colorName = (StringNames)(1000000 + AllCustomColors.Count);
            AllCustomColors.Add(this);
        }
        public Color32 Color;
        public Color32 ShadowColor;
        public string ColorName;
        public StringNames colorName;
    }
}
