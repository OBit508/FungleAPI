using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using BepInEx;
using BepInEx.Unity.IL2CPP;
using HarmonyLib;

namespace FungleAPI.ModCompatibility
{
    public static class LevelImpostorSupport
    {
        public static Assembly LevelImpostorAssembly;
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
