using FungleAPI.Cosmetics.Hats;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FungleAPI.Cosmetics.Patches
{
    [HarmonyPatch(typeof(HatManager), nameof(HatManager.Initialize))]
    internal static class HatManagerPatch
    {
        public static void Postfix(HatManager __instance)
        {
            HatData emptyHat = __instance.allHats.First();
            IEnumerable<HatData> vanillaHats = __instance.allHats.Where(h => !h.IsEmpty);

            __instance.allHats = null;

            List<HatData> hats = new List<HatData>() { emptyHat };
            foreach (CustomHat customHat in CosmeticManager.AllHats)
            {
                hats.Add(customHat.HatData);
            }
            __instance.allHats = hats.ToArray();
            hats = null;

            __instance.allHats = __instance.allHats.Concat(vanillaHats).ToArray();
        }
    }
}
