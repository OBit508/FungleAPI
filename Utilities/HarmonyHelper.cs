using FungleAPI.Base.Roles;
using FungleAPI.Configuration.Attributes;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace FungleAPI.Utilities
{
    public static class HarmonyHelper
    {
        public static Dictionary<MethodBase, Func<object>> Patches = new Dictionary<MethodBase, Func<object>>();
        public static void PatchAllOverrideMethods(IEnumerable<Type> types, Type baseType, string methodName, MethodInfo method)
        {
            foreach (Type type in types)
            {
                if (baseType.IsAssignableFrom(type))
                {
                    foreach (MethodInfo m in baseType.GetMethods())
                    {
                        if (m.Name == methodName)
                        {
                            FungleAPIPlugin.Harmony.Patch(m, new HarmonyMethod(method));
                        }
                    }
                }
            }
        }
        public static void Remove_FungleAPI_HarmonyLib_Patch(MethodInfo original, string TypeName, string MethodName)
        {
            Type type = FungleAPIPlugin.Plugin.AllTypes.FirstOrDefault(t => t.Name == TypeName);
            if (type != null)
            {
                MethodInfo methodInfo = type.GetMethod(MethodName, AccessTools.all);
                if (methodInfo != null)
                {
                    FungleAPIPlugin.Harmony.Unpatch(original, methodInfo);
                }
            }
        }
        public static bool GetPrefix(MethodBase __originalMethod, ref object __result)
        {
            __result = Patches[__originalMethod]();
            return false;
        }
    }
}
