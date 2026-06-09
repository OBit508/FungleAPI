using FungleAPI.Attributes;
using FungleAPI.Hud;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace FungleAPI.Components
{
    [RegisterTypeInIl2Cpp]
    public class CustomAbilityButtonHelper : MonoBehaviour
    {
        public CustomAbilityButton Button;

        public void Update()
        {
            Button.Update();
        }
    }
}
