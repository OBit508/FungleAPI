using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace FungleAPI.Cosmetics.Helpers
{
    public class CustomColor
    {
        public CustomColor(Color color, Color backColor, StringNames colorName)
        {
            Color = color;
            BackColor = backColor;
            ColorName = colorName;
        }
        public int ColorId { get; internal set; }
        public Color Color;
        public Color BackColor;
        public StringNames ColorName;
    }
}
