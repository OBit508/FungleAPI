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
        [HarmonyPatch("GetStringWithDefault", new Type[] { typeof(StringNames), typeof(string), typeof(Il2CppReferenceArray<Il2CppSystem.Object>) })]
        [HarmonyPatch("GetString", new Type[] { typeof(StringNames), typeof(Il2CppReferenceArray<Il2CppSystem.Object>) })]
        [HarmonyPrefix]
        public static bool GetPrefix([HarmonyArgument(0)] StringNames id, [HarmonyArgument(1)] Il2CppReferenceArray<Il2CppSystem.Object> parts, ref string __result)
        {
            foreach (Translator s in Translator.All)
            {
                if (s.StringName == id)
                {
                    __result = s.GetString();
                    return false;
                }
            }
            return !ReactorSupport.LocalizationManager_TryGetTextFormatted(id, parts, out __result);
        }
    }
}
