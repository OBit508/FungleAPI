using FungleAPI.ModCompatibility;
using FungleAPI.ModCompatibility.ReactorSupportTemp;
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
            if (ReactorCompatibility.Instance != null) return;

            if (modsText == null)
            {
                modsText = "";
                ModPlugin last = ModPluginManager.AllPlugins.Last();
                foreach (ModPlugin plugin in ModPluginManager.AllPlugins)
                {
                    modsText += $"{(plugin != FungleAPIPlugin.Plugin ? plugin.ModName : "FungleAPI")} {plugin.ModVersion}";
                    if (plugin != last)
                    {
                        modsText += ", ";
                    }
                }
            }
            __instance.text.enableWordWrapping = false;
            __instance.text.horizontalAlignment = TMPro.HorizontalAlignmentOptions.Center;
            __instance.text.text += "\n<align=center><size=50%>" + modsText + "</size></align>";
        }
    }
}
