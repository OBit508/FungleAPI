using Epic.OnlineServices;
using FungleAPI.Roles;
using FungleAPI.Rpc;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace FungleAPI.Configuration
{
    public static class ConfigurationManager
    {
        public static Dictionary<MethodBase, CustomConfig> Configs = new Dictionary<MethodBase, CustomConfig>();
        public static List<CustomConfig> InitializeConfigs(object obj)
        {
            Type type = obj.GetType();
            List<CustomConfig> configs = new List<CustomConfig>();
            foreach (PropertyInfo property in type.GetProperties())
            {
                CustomConfig att = (CustomConfig)property.GetCustomAttribute(typeof(CustomConfig));
                if (att != null)
                {
                    att.Initialize(type, property, obj);
                    MethodInfo method = property.GetGetMethod(true);
                    if (method != null)
                    {
                        Configs.Add(method, att);
                        FungleAPIPlugin.Harmony.Patch(method, new HarmonyMethod(typeof(ConfigurationManager).GetMethod("GetPrefix", BindingFlags.Static | BindingFlags.Public, null, new Type[] { typeof(MethodBase), property.PropertyType.MakeByRefType() }, null)));
                    }
                    configs.Add(att);
                }
            }
            return configs;
        }
        public static bool GetPrefix(MethodBase __originalMethod, ref float __result)
        {
            __result = float.Parse(Configs[__originalMethod].GetValue());
            return false;
        }
        public static bool GetPrefix(MethodBase __originalMethod, ref bool __result)
        {
            __result = bool.Parse(Configs[__originalMethod].GetValue());
            return false;
        }
        public static bool GetPrefix(MethodBase __originalMethod, ref string __result)
        {
            __result = Configs[__originalMethod].GetValue();
            return false;
        }
    }
}
