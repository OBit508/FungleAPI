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
        internal static List<AspectPosition> Bottom = new List<AspectPosition>();
        internal static HudUpdateFlag UpdateFlag = HudUpdateFlag.OnSetHudActive;
        internal static float UpdateDelay;
        internal static bool Active;
        public static Transform BottomLeft;
        public static Transform BottomRight;
        public static bool IsActive => Active;
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
        public static void SetBottomSize(float size)
        {
            float distance = 1 - size;
            if (distance != 0)
            {
                distance = distance / 2;
            }

            Vector2 edgeDistance = new Vector3(0.8f - distance, 0.7f - distance, 0);
            Vector3 scale = Vector3.one * size;

            foreach (AspectPosition aspectPosition in Bottom)
            {
                aspectPosition.transform.localScale = scale;

                aspectPosition.DistanceFromEdge = edgeDistance;
                aspectPosition.AdjustPosition();

                GridArrange gridArrange = aspectPosition.GetComponent<GridArrange>();
                gridArrange.CellSize = scale;

                if (gridArrange.cells != null && gridArrange.cells.Count > 0) gridArrange.ArrangeChilds();
            }
        }
        public static void SetZoom(float zoom)
        {
            HudManager hudManager = HudManager.Instance;
            if (hudManager == null) return;

            Camera.main.orthographicSize = zoom;
            hudManager.UICamera.orthographicSize = zoom;

            foreach (AspectPosition aspectPosition in hudManager.GetComponentsInChildren<AspectPosition>(true))
            {
                aspectPosition.AdjustPosition();
            }
        }
        public static void RegisterButton(Type type, ModPlugin plugin)
        {
            CustomAbilityButton button = (CustomAbilityButton)Activator.CreateInstance(type);
            Buttons.Add(type, button);
            plugin.BasePlugin.Log.LogInfo("Registered CustomButton " + type.Name);
        }
    }
}
