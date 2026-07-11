using FungleAPI.Api;
using FungleAPI.Base.Roles;
using BepInEx.Unity.IL2CPP;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace FungleAPI.Utilities.Harmony
{
    public static class HarmonyHelper
    {
        public static Dictionary<MethodBase, Func<object>> Patches = new Dictionary<MethodBase, Func<object>>();
        public static void Remove_FungleAPI_HarmonyLib_Patch(MethodInfo original, string TypeName, string MethodName)
        {
            Type type = FungleApiPlugin.Plugin.AllTypes.FirstOrDefault(t => t.Name == TypeName);
            if (type != null)
            {
                MethodInfo methodInfo = type.GetMethod(MethodName, AccessTools.all);
                if (methodInfo != null)
                {
                    FungleApiPlugin.Harmony.Unpatch(original, methodInfo);
                }
            }
        }
        public static bool GetPrefix(MethodBase __originalMethod, ref object __result)
        {
            __result = Patches[__originalMethod]();
            return false;
        }
        public static bool IsPluginLoaded(string pluginId)
        {
            return IL2CPPChainloader.Instance?.Plugins.ContainsKey(pluginId) == true;
        }
        public static bool PatchIfPluginLoaded(string pluginId, MethodBase original, HarmonyMethod prefix = null, HarmonyMethod postfix = null)
        {
            if (!IsPluginLoaded(pluginId))
            {
                return false;
            }
            FungleApiPlugin.Harmony.Patch(original, prefix, postfix);
            return true;
        }
    }
}
