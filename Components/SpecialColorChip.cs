using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FungleAPI.Cosmetics.Helpers;
using UnityEngine;

namespace FungleAPI.Components
{
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
