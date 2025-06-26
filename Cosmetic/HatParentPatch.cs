using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FungleAPI.Cosmetic
{
    [HarmonyPatch(typeof(HatParent), "LateUpdate")]
    class HatParentPatch
    {
        public static void Postfix(HatParent __instance)
        {
            if (__instance.Hat != null)
            {
                foreach (CustomHat hat in CustomHat.AllCultomHats)
                {
                    if (hat.hatData == __instance.Hat)
                    {
                        __instance.FrontLayer.sprite = hat.Sprite;
                        __instance.BackLayer.sprite = hat.Sprite;
                    }
                }
            }
        }
    }
}
