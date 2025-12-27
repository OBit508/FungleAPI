using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AmongUs.Data;
using FungleAPI.Components;
using FungleAPI.Cosmetics.Helpers;
using FungleAPI.Utilities;
using FungleAPI.Utilities.Assets;
using HarmonyLib;
using UnityEngine;

namespace FungleAPI.Cosmetics.Patches
{
    [HarmonyPatch(typeof(PlayerTab))]
    internal static class PlayerTabPatch
    {
        public static BoxCollider2D Collider;
        [HarmonyPatch("OnEnable")]
        [HarmonyPostfix]
        public static void OnEnablePostfix(PlayerTab __instance)
        {
            if (__instance.scroller == null)
            {
                Scroller newScroller = GameObject.Instantiate<Scroller>(PlayerCustomizationMenu.Instance.Tabs[1].Tab.scroller, __instance.transform, true);
                newScroller.Inner.transform.DestroyChildren();
                GameObject gameObject = new GameObject();
                gameObject.layer = 5;
                gameObject.name = "SpriteMask";
                gameObject.transform.SetParent(__instance.transform);
                gameObject.transform.localPosition = new Vector3(0f, 0f, 0f);
                gameObject.transform.localScale = new Vector3(500f, 4.76f);
                gameObject.AddComponent<SpriteMask>().sprite = FungleAssets.Empty;
                Collider = gameObject.AddComponent<BoxCollider2D>();
                Collider.size = new Vector2(1f, 0.75f);
                Collider.enabled = true;
                __instance.scroller = newScroller;
                for (int i = 0; i < __instance.ColorChips.Count; i++)
                {
                    ColorChip colorChip = __instance.ColorChips[i];
                    colorChip.transform.SetParent(__instance.scroller.Inner);
                    colorChip.Button.ClickMask = Collider;
                    colorChip.Inner.FrontLayer.maskInteraction = SpriteMaskInteraction.VisibleInsideMask;
                    if (CosmeticManager.IsSpecialColor(i, out SpecialColor color))
                    {
                        SpecialColorChip specialColorChip = colorChip.gameObject.GetOrAddComponent<SpecialColorChip>();
                        specialColorChip.Color = color;
                        specialColorChip.Chip = colorChip;
                    }
                }
                __instance.SetScrollerBounds();
            }
        }
    }
}
