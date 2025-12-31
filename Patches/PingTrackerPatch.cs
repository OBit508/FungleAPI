using FungleAPI.ModCompatibility;
using FungleAPI.PluginLoading;
using HarmonyLib;
using Newtonsoft.Json.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FungleAPI.Patches
{
    [HarmonyPatch(typeof(PingTracker), "Update")]
    internal static class PingTrackerPatch
    {
        public static string modsText;
        public static void Postfix(PingTracker __instance)
        {
            if (modsText == null)
            {
                modsText = "";
                foreach (ModPlugin plugin in ModPlugin.AllPlugins)
                {
                    modsText += (plugin == ModPlugin.AllPlugins[0] ? "" : ", ") + plugin.ModCredits;
                }
            }
            __instance.text.enableWordWrapping = false;
            __instance.text.horizontalAlignment = TMPro.HorizontalAlignmentOptions.Center;
            __instance.text.text += "\n<size=2>" + modsText + "</size>";
            string extraText = ReactorSupport.ReactorCreditsText;
            if (extraText != null)
            {
                __instance.text.text += "\n" + extraText;
            }
        }
    }
}
