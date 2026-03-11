using BepInEx;
using BepInEx.Unity.IL2CPP;
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
        public static PropertyInfo Instance;
        public static FieldInfo IsInit;
        public static void Initialize()
        {
            if (IL2CPPChainloader.Instance.Plugins.TryGetValue("com.DigiWorm.LevelImposter", out PluginInfo pluginInfo))
            {
                FungleAPIPlugin.Instance.Log.LogInfo("Initializing LevelImpostor Support");
                LevelImpostorAssembly = pluginInfo.Instance.GetType().Assembly;
                Type type = LevelImpostorAssembly.GetType("LevelImposter.DB.AssetDB");
                Instance = type.GetProperty("Instance");
                IsInit = type.GetField("_isInit", AccessTools.all);
                LIShipStatus = LevelImpostorAssembly.GetType("LevelImposter.Core.LIShipStatus");
            }
        }
        public static System.Collections.IEnumerator CoUnsafeWaitForMapLoading()
        {
            if (Instance != null)
            {
                while (Instance.GetValue(null) == null)
                {
                    yield return null;
                }
                while (!(bool)IsInit.GetValue(Instance.GetValue(null)))
                {
                    yield return null;
                }
            }
        }
    }
}
