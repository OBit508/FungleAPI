using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FungleAPI.Cosmetic
{
    [HarmonyPatch(typeof(HatManager), "Initialize")]
    class HatManagerPatch
    {
        public static void Postfix(HatManager __instance)
        {
            List<HatData> list = new List<HatData>() { __instance.allHats[0] };
            foreach (CustomHat hat in CustomHat.AllCultomHats)
            {
                list.Add(hat.hatData);
                hat.hatData.PreviewData = __instance.allHats[1].PreviewData;
                hat.hatData.ViewDataRef = __instance.allHats[1].ViewDataRef;
            }
            for (int i = 1; i < __instance.allHats.Count; i++)
            {
                list.Add(__instance.allHats[i]);
            }
            __instance.allHats = list.ToArray();
            List<VisorData> list3 = new List<VisorData>() { __instance.allVisors[0] };
            foreach (CustomVisor visor in CustomVisor.AllCustomVisors)
            {
                list3.Add(visor.VisorData);
                visor.VisorData.PreviewData = __instance.allVisors[1].PreviewData;
                visor.VisorData.ViewDataRef = __instance.allVisors[1].ViewDataRef;
            }
            for (int i = 1; i < __instance.allVisors.Count; i++)
            {
                list3.Add(__instance.allVisors[i]);
            }
            __instance.allVisors = list3.ToArray();
        }
    }
}
