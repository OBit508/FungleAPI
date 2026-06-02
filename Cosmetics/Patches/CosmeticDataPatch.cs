using FungleAPI.Utilities;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FungleAPI.Cosmetics.Patches
{
    [HarmonyPatch(typeof(CosmeticData), nameof(CosmeticData.GetItemName))]
    internal static class CosmeticDataPatch
    {
        public static void Postfix(CosmeticData __instance, ref string __result)
        {
            if (CosmeticManager.CosmeticsNames.TryGetValue(__instance, out StringNames stringNames))
            {
                __result = stringNames.GetString();
            }
        }
    }
}
