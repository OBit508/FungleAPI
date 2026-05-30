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
using static Il2CppSystem.Xml.Schema.FacetsChecker.FacetsCompiler;

namespace FungleAPI.ModCompatibility
{
    /// <summary>
    /// This class was created to allow LevelImpostor to load map prefabs independently without FungleAPI interference
    /// </summary>
    public static class LevelImpostorSupport
    {
        public static Assembly LevelImpostorAssembly;
        public static Type LIShipStatus;
        public static PropertyInfo IsInit;
        private static bool IsLoaded;

        [EventRegister]
        public static void Initialize(FinishedPluginLoadingEvent finishedPluginLoadingEvent)
        {
            if (IL2CPPChainloader.Instance.Plugins.TryGetValue("com.DigiWorm.LevelImposter", out PluginInfo pluginInfo))
            {
                LevelImpostorAssembly = pluginInfo.Instance.GetType().Assembly;
                Type type = LevelImpostorAssembly.GetType("LevelImposter.DB.AssetDB");
                IsInit = type.GetProperty("IsInit", AccessTools.all);
                LIShipStatus = LevelImpostorAssembly.GetType("LevelImposter.Core.LIShipStatus");
            }
        }
        public static System.Collections.IEnumerator CoWaitMapLoading()
        {
            if (IsLoaded) yield break;

            while (!(bool)IsInit.GetValue(null))
            {
                yield return null;
            }
            IsLoaded = true;
        }
    }
}
