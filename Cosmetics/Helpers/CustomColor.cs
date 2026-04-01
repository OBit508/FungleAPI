using FungleAPI.Translation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace FungleAPI.Cosmetics.Helpers
{
    /// <summary>
    /// Base class to create a color
    /// </summary>
    public class CustomColor
    {
        public CustomColor(Color playerColor, Color backColor, StringNames colorName)
        {
            PlayerColor = playerColor;
            BackColor = backColor;
            ColorName = colorName;
        }
        public CustomColor(Color playerColor, Color backColor, string colorName)
        {
            PlayerColor = playerColor;
            BackColor = backColor;
            ColorName = new Translator(colorName).StringName;
        }
        public int ColorId { get; internal set; }
        public Color PlayerColor;
        public Color BackColor;
        public StringNames ColorName;
    }
}
