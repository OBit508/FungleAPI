using FungleAPI.Cosmetics.Helpers;
using Il2CppMono;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace FungleAPI.Components
{
    /// <summary>
    /// The component used to create animated colors
    /// </summary>
    [Attributes.RegisterTypeInIl2Cpp]
    public class SpecialColorChip : MonoBehaviour
    {
        public ColorChip Chip;
        public SpecialColor Color;
        public void Update()
        {
            Chip.Inner.SpriteColor = Color.BaseColor;
        }
    }
}
