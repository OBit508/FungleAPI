using FungleAPI.MonoBehaviours;
using HarmonyLib;
using PowerTools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FungleAPI.Cosmetic
{
    [HarmonyPatch(typeof(VisorLayer), "UpdateMaterial")]
    class VisorLayerPatch
    {
        public static void Postfix(VisorLayer __instance)
        {
            if (__instance.GetComponent<Updater>() == null)
            {
                __instance.gameObject.AddComponent<Updater>().onUpdate = new Action(delegate
                {
                    foreach (CustomVisor visor in CustomVisor.AllCustomVisors)
                    {
                        if (visor.VisorData == __instance.visorData)
                        {
                            __instance.Image.sprite = visor.Sprite;
                        }
                    }
                });
            }
        }
    }
}
