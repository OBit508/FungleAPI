using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace FungleAPI.Utilities
{
    /// <summary>
    /// Extensions for the colors
    /// </summary>
    public static class ColorExtensions
    {
        /// <summary>
        /// Returns the color but more darken
        /// </summary>
        public static Color Darken(this Color color, float num = 0.5f)
        {
            return Color.Lerp(color, Color.black, num);
        }
        /// <summary>
        /// Returns the color but more lighter
        /// </summary>
        public static Color Lighten(this Color color, float num = 0.5f)
        {
            return Color.Lerp(color, Color.white, num);
        }
        /// <summary>
        /// Retuns the color mixed with the given color
        /// </summary>
        public static Color Mix(this Color a, Color b, float num = 0.5f)
        {
            return Color.Lerp(a, b, num);
        }
        /// <summary>
        /// Retuns the inverted color of this color
        /// </summary>
        public static Color Invert(this Color color)
        {
            return new Color(-color.r, -color.g, -color.b);
        }
    }
}
