using FungleAPI.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace FungleAPI.Hud
{
    public static class HudHelper
    {
        internal static Dictionary<Type, CustomAbilityButton> Buttons = new Dictionary<Type, CustomAbilityButton>();
        internal static HudUpdateFlag UpdateFlag = HudUpdateFlag.OnSetHudActive;
        internal static float UpdateDelay;
        internal static bool Active;
        public static Transform BottomLeft;
        public static Transform BottomRight;
        public static void SetUpdateFlag(HudUpdateFlag flag, float delay = 0)
        {
            UpdateFlag = flag;
            UpdateDelay = delay;
        }
        public static T GetButtonInstance<T>() where T : CustomAbilityButton
        {
            if (Buttons.TryGetValue(typeof(T), out CustomAbilityButton button))
            {
                return button.SimpleCast<T>();
            }
            return null;
        }
    }
}
