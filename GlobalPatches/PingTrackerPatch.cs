using FungleAPI.Api;
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
                modsText = $"FungleAPI {FungleApiPlugin.ModV}";

                List<ModPlugin> plugins = ModPluginManager.AllPlugins.FindAll(p => p.FunglePlugin.Credits != null && p != FungleApiPlugin.Plugin);

                if (plugins.Count > 0)
                {
                    modsText += ", ";

                    ModPlugin last = plugins.Last();
                    foreach (ModPlugin plugin in plugins)
                    {
                        PluginCredits pluginCredits = plugin.FunglePlugin.Credits.Value;

                        modsText += $"{pluginCredits.Name} {pluginCredits.Version}";
                        if (plugin != last)
                        {
                            modsText += ", ";
                        }
                    }
                }
            }
            __instance.text.enableWordWrapping = false;
            __instance.text.horizontalAlignment = TMPro.HorizontalAlignmentOptions.Center;
            __instance.text.text += "\n<align=center><size=50%>" + modsText + "</size></align>";
        }
    }
}
