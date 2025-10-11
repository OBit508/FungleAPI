using FungleAPI.Utilities.Assets;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FungleAPI.Patches
{
    [HarmonyPatch(typeof(MainMenuManager), "Start")]
    internal static class MainMenuManagerPatch
    {
        public static void Postfix(MainMenuManager __instance)
        {
            FungleAssets.CreditsPrefab.Instantiate(__instance.transform.GetChild(5).GetChild(1).GetChild(3));
        }
    }
}
