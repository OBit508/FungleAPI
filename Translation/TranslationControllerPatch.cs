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
    [HarmonyPatch(typeof(TranslationController), "GetString", new Type[]
   {
        typeof(StringNames),
        typeof(Il2CppReferenceArray<Il2CppSystem.Object>)
   })]
    public class TranslationControllerPatch
    {
        public static bool Prefix(ref string __result, [HarmonyArgument(0)] StringNames name)
        {
            foreach (Translator s in Translator.All)
            {
                if (s.StringName == name)
                {
                    __result = s.GetString();
                    return false;
                }
            }
            return true;
        }
    }
}
