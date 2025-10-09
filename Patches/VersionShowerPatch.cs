using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;
using FungleAPI;
using FungleAPI.ModCompatibility;

namespace FungleAPI.Patches
{
    [HarmonyPatch(typeof(VersionShower), "Start")]
    internal static class VersionShowerPatch
    {
        public static void Postfix(VersionShower __instance)
        {
            string mciText = !FungleAPIPlugin.MCIActive ? "" : " - MCI " + MCIUtils.GetMCIVersion();
            __instance.text.text = "AmongUs " + __instance.text.text + " - FungleAPI " + FungleAPIPlugin.ModV + mciText;
        }
    }
}
