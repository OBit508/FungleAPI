using AmongUs.Data;
using AmongUs.Data.Player;
using FungleAPI.Patches;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace FungleAPI.Cosmetic
{    class Tabs
    {
        [HarmonyPatch(typeof(HatsTab))]
        class HatTabPatch
        {
            [HarmonyPatch("Update")]
            [HarmonyPostfix]
            public static void OnUpdate(HatsTab __instance)
            {
                foreach (ColorChip hatChip in __instance.ColorChips)
                {
                    foreach (CustomHat hat in CustomHat.AllCultomHats)
                    {
                        if (hat.hatData == hatChip.Tag.Cast<HatData>())
                        {
                            hatChip.Inner.FrontLayer.sprite = hat.Sprite;
                            hatChip.Inner.BackLayer.sprite = hat.Sprite;
                        }
                        if (hat.hatData == __instance.currentHat)
                        {
                            PlayerCustomizationMenu.Instance.SetItemName(hat.HatName);
                        }
                    }
                }
            }
        }
        [HarmonyPatch(typeof(VisorsTab), "Update")]
        class VisorTabPatch
        {
            public static void Postfix(VisorsTab __instance)
            {
                foreach (ColorChip visorChip in __instance.ColorChips)
                {
                    foreach (CustomVisor visor in CustomVisor.AllCustomVisors)
                    {
                        if (visor.VisorData.ProductId == visorChip.ProductId)
                        {
                            visorChip.Inner.FrontLayer.sprite = visor.Sprite;
                            visorChip.Inner.BackLayer.sprite = visor.Sprite;
                        }
                        if (visor.VisorData.ProductId == __instance.visorId)
                        {
                            PlayerCustomizationMenu.Instance.SetItemName(visor.VisorName);
                        }
                    }
                }
            }
        }
    }
}
