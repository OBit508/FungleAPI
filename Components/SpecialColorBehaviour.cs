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
