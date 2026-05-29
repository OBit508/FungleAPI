using FungleAPI.ModCompatibility;
using FungleAPI.ModCompatibility.ReactorSupportTemp;
using FungleAPI.Patches;
using HarmonyLib;
using Il2CppInterop.Runtime.InteropTypes.Arrays;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FungleAPI.Translation
{
    [HarmonyPatch(typeof(TranslationController))]
    internal static class TranslationControllerPatch
    {
        [HarmonyPatch("GetString", new Type[] { typeof(StringNames), typeof(Il2CppReferenceArray<Il2CppSystem.Object>) })]
        [HarmonyPostfix]
        public static void GetStringPostfix(StringNames id, Il2CppReferenceArray<Il2CppSystem.Object> parts, ref string __result)
        {
            if (ReactorCompatibility.Instance != null) return;

            if (TranslationManager.Translators.TryGetValue(id, out Translator translator))
            {
                __result = translator.GetString();
            }
        }
        [HarmonyPatch("GetStringWithDefault", new Type[] { typeof(StringNames), typeof(string), typeof(Il2CppReferenceArray<Il2CppSystem.Object>) })]
        [HarmonyPostfix]
        public static void GetStringWithDefaultPostfix(StringNames id, string defaultStr, Il2CppReferenceArray<Il2CppSystem.Object> parts, ref string __result)
        {
            if (ReactorCompatibility.Instance != null) return;

            if (TranslationManager.Translators.TryGetValue(id, out Translator translator))
            {
                __result = translator.GetString();
            }
        }
    }
}
