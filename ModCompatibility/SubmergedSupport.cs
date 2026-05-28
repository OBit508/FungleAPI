using BepInEx;
using BepInEx.Unity.IL2CPP;
using FungleAPI.Event;
using FungleAPI.Event.BelpInEx;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace FungleAPI.ModCompatibility
{
    /// <summary>
    /// This class was created to allow Submerged to work together with FungleAPI
    /// </summary>
    public static class SubmergedSupport
    {
        public static Assembly SubmergedAssembly;
        public static Type SubmarineStatus;

        [EventRegister]
        public static void Initialize(FinishedPluginLoadingEvent finishedPluginLoadingEvent)
        {
            if (IL2CPPChainloader.Instance.Plugins.TryGetValue("Submerged", out PluginInfo pluginInfo))
            {
                FungleAPIPlugin.Instance.Log.LogInfo("Initializing Submerged Support");
                SubmergedAssembly = pluginInfo.Instance.GetType().Assembly;
                SubmarineStatus = SubmergedAssembly.GetType("Submerged.Map.SubmarineStatus");
            }
        }
    }
}
