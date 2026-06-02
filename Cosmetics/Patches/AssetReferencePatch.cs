using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.AddressableAssets;

namespace FungleAPI.Cosmetics.Patches
{
    [HarmonyPatch(typeof(AssetReference), nameof(AssetReference.RuntimeKeyIsValid))]
    internal static class AssetReferencePatch
    {
        public static bool Prefix(AssetReference __instance, ref bool __result)
        {
            if (CosmeticManager.Assets.TryGetValue(__instance.AssetGUID, out _))
            {
                __result = true;
                return false;
            }
            return true;
        }
    }
}
