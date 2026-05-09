using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FungleAPI.Patches
{
    [HarmonyPatch(typeof(ModManager), "LateUpdate")]
    internal static class ModManagerPatch
    {
        public static bool show;
        public static void Postfix(ModManager __instance)
        {
            if (!show)
            {
                __instance.ShowModStamp();
                show = true;
            }
        }
    }
}
