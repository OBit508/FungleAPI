using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AmongUs.Data;
using FungleAPI.Components;
using FungleAPI.Cosmetics.Helpers;
using FungleAPI.Utilities;
using HarmonyLib;
using UnityEngine;

namespace FungleAPI.Cosmetics.Patches
{
    [HarmonyPatch(typeof(PlayerTab))]
    internal static class PlayerTabPatch
    {
        [HarmonyPatch("OnEnable")]
        [HarmonyPrefix]
        public static bool OnEnablePrefix(PlayerTab __instance)
        {
            __instance.PlayerPreview.gameObject.SetActive(true);
            if (__instance.HasLocalPlayer())
            {
                __instance.PlayerPreview.UpdateFromLocalPlayer(PlayerMaterial.MaskType.None);
            }
            else
            {
                __instance.PlayerPreview.UpdateFromDataManager(PlayerMaterial.MaskType.None);
            }
            float num = (float)Palette.PlayerColors.Length / 4f;
            for (int i = 0; i < Palette.PlayerColors.Length; i++)
            {
                float x = __instance.XRange.Lerp((float)(i % 4) / 3f);
                float y = __instance.YStart - (float)(i / 4) * __instance.YOffset;
                ColorChip colorChip = UnityEngine.Object.Instantiate<ColorChip>(__instance.ColorTabPrefab);
                colorChip.transform.SetParent(__instance.ColorTabArea);
                colorChip.transform.localPosition = new Vector3(x, y, -1f);
                int j = i;
                if (ActiveInputManager.currentControlType == ActiveInputManager.InputType.Keyboard)
                {
                    colorChip.Button.OnMouseOver.AddListener(new Action(delegate
                    {
                        __instance.SelectColor(j);
                    }));
                    colorChip.Button.OnMouseOut.AddListener(new Action(delegate
                    {
                        __instance.SelectColor(__instance.GetDisplayColor());
                    }));
                    colorChip.Button.SetNewAction(delegate
                    {
                        __instance.ClickEquip();
                    });
                }
                else
                {
                    colorChip.Button.SetNewAction(delegate
                    {
                        __instance.SelectColor(j);
                    });
                }
                colorChip.Inner.SpriteColor = Palette.PlayerColors[i];
                if (CosmeticManager.IsSpecialColor(i, out SpecialColor specialColor))
                {
                    SpecialColorChip specialColorChip = colorChip.gameObject.GetOrAddComponent<SpecialColorChip>();
                    specialColorChip.Chip = colorChip;
                    specialColorChip.Color = specialColor;
                }
                colorChip.Tag = j;
                __instance.ColorChips.Add(colorChip);
            }
            __instance.currentColor = (int)DataManager.Player.Customization.Color;
            return false;
        }
    }
}
