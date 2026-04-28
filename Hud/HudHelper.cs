using FungleAPI.PluginLoading;
using FungleAPI.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Channels;
using System.Threading.Tasks;
using UnityEngine;

namespace FungleAPI.Hud
{
    /// <summary>
    /// Helps the hud system
    /// </summary>
    public static class HudHelper
    {
        internal static Dictionary<Type, CustomAbilityButton> Buttons = new Dictionary<Type, CustomAbilityButton>();
        internal static HudUpdateFlag UpdateFlag = HudUpdateFlag.OnSetHudActive;
        internal static float UpdateDelay;
        internal static bool Active;
        public static Transform BottomLeft;
        public static Transform BottomRight;
        /// <summary>
        /// Changes when the Hud buttons need to be updated
        /// </summary>
        public static void SetUpdateFlag(HudUpdateFlag flag, float delay = 0)
        {
            UpdateFlag = flag;
            UpdateDelay = delay;
        }
        /// <summary>
        /// Returns the instance of the given type
        /// </summary>
        public static T GetButtonInstance<T>() where T : CustomAbilityButton
        {
            return GetButtonInstance(typeof(T))?.SimpleCast<T>() ?? null;
        }
        /// <summary>
        /// Returns the instance of the given type
        /// </summary>
        public static CustomAbilityButton GetButtonInstance(Type type)
        {
            if (Buttons.TryGetValue(type, out CustomAbilityButton button))
            {
                return button;
            }
            return null;
        }
        public static void RegisterButton(Type type, ModPlugin plugin)
        {
            CustomAbilityButton button = (CustomAbilityButton)Activator.CreateInstance(type);
            Buttons.Add(type, button);
            plugin.BasePlugin.Log.LogInfo("Registered CustomButton " + type.Name);
        }
    }
}
