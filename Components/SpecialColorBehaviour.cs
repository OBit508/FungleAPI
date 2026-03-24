using FungleAPI.Attributes;
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
    [RegisterTypeInIl2Cpp]
    public class SpecialColorBehaviour : MonoBehaviour
    {
        public SpecialColor Color;
        public Material Mat;
        public void Update()
        {
            if (Color != null && Mat != null)
            {
                Color.UpdateMaterial(Mat);
            }
        }
    }
}
