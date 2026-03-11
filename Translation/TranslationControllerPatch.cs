using FungleAPI.ModCompatibility;
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
        [HarmonyPrefix]
        public static bool GetStringPrefix(StringNames id, Il2CppReferenceArray<Il2CppSystem.Object> parts, ref string __result)
        {
            if (TranslationManager.Translators.TryGetValue(id, out Translator translator))
            {
                __result = translator.GetString();
                return false;
            }
            return !ReactorSupport.LocalizationManager_TryGetTextFormatted(id, parts, out __result);
        }
        [HarmonyPatch("GetStringWithDefault", new Type[] { typeof(StringNames), typeof(string), typeof(Il2CppReferenceArray<Il2CppSystem.Object>) })]
        [HarmonyPrefix]
        public static bool GetStringWithDefaultPrefix( StringNames id, string defaultStr, Il2CppReferenceArray<Il2CppSystem.Object> parts, ref string __result)
        {
            if (TranslationManager.Translators.TryGetValue(id, out Translator translator))
            {
                __result = translator.GetString();
                return false;
            }
            return !ReactorSupport.LocalizationManager_TryGetTextFormatted(id, parts, out __result);
        }
    }
}
