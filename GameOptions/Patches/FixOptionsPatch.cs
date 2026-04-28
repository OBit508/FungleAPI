using FungleAPI.Utilities;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FungleAPI.GameOptions.Patches
{
    internal static class FixOptionsPatch
    {
        [HarmonyPatch(typeof(StringOption))]
        public static class String
        {
            [HarmonyPatch("Initialize")]
            [HarmonyPrefix]
            internal static bool InitializePrefix(StringOption __instance)
            {
                if (__instance.name == "ModdedOption")
                {
                    __instance.TitleText.text = DestroyableSingleton<TranslationController>.Instance.GetString(__instance.Title);
                    __instance.ValueText.text = DestroyableSingleton<TranslationController>.Instance.GetString(__instance.Values[__instance.Value]);
                    __instance.AdjustButtonsActiveState();
                    return false;
                }
                return true;
            }
            [HarmonyPatch("UpdateValue")]
            [HarmonyPrefix]
            internal static bool UpdateValuePrefix(StringOption __instance)
            {
                return __instance.name != "ModdedOption";
            }
        }
        [HarmonyPatch(typeof(NumberOption))]
        public static class Number
        {
            [HarmonyPatch("Initialize")]
            [HarmonyPrefix]
            public static bool InitializePrefix(NumberOption __instance)
            {
                if (__instance.name == "ModdedOption")
                {
                    __instance.AdjustButtonsActiveState();
                    __instance.TitleText.text = DestroyableSingleton<TranslationController>.Instance.GetString(__instance.Title);
                    __instance.Value = __instance.Data.SafeCast<FloatGameSetting>().Value;
                    return false;
                }
                return true;
            }
            [HarmonyPatch("UpdateValue")]
            [HarmonyPrefix]
            public static bool UpdateValuePrefix(NumberOption __instance)
            {
                return __instance.name != "ModdedOption";
            }
        }
        [HarmonyPatch(typeof(ToggleOption))]
        public static class Toggle
        {
            [HarmonyPatch("Initialize")]
            [HarmonyPrefix]
            public static bool InitializePrefix(ToggleOption __instance)
            {
                if (__instance.name == "ModdedOption")
                {
                    __instance.TitleText.text = DestroyableSingleton<TranslationController>.Instance.GetString(__instance.Title);
                    return false;
                }
                return true;
            }
            [HarmonyPatch("UpdateValue")]
            [HarmonyPrefix]
            public static bool UpdateValuePrefix(ToggleOption __instance)
            {
                return __instance.name != "ModdedOption";
            }
        }
    }
}
