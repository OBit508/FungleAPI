using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FungleAPI.Cosmetics.Helpers;
using UnityEngine;

namespace FungleAPI.Cosmetics.Helpers
{
    public class SpecialColor : CustomColor
    {
        public SpecialColor(Color color, Color backColor, StringNames colorName)
            : base(color, backColor, colorName) { }
        public virtual void UpdateMaterial(Material material) { }
        public virtual Color BaseColor => Color.black;
    }
}
