using Epic.OnlineServices;
using FungleAPI.Roles;
using FungleAPI.Networking;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using FungleAPI.Role;

namespace FungleAPI.Configuration
{
    public static class ConfigurationManager
    {
        public static Dictionary<MethodBase, ModdedOption> Configs = new Dictionary<MethodBase, ModdedOption>();
        internal static Dictionary<MethodBase, RoleConfig> RoleConfigs = new Dictionary<MethodBase, RoleConfig>();
        public static List<ModdedOption> InitializeConfigs(object obj)
        {
            Type type = obj.GetType();
            List<ModdedOption> configs = new List<ModdedOption>();
            foreach (PropertyInfo property in type.GetProperties())
            {
                ModdedOption att = (ModdedOption)property.GetCustomAttribute(typeof(ModdedOption));
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
        internal static void PatchRoleConfig(Type type, RoleConfig config)
        {
            PropertyInfo property = type.GetProperty("Configuration");
            if (property != null && property.PropertyType == typeof(RoleConfig))
            {
                RoleConfigs.Add(property.GetGetMethod(), config);
                FungleAPIPlugin.Harmony.Patch(property.GetGetMethod(), new HarmonyMethod(typeof(ConfigurationManager).GetMethod("GetRoleConfigPrefix", BindingFlags.Static | BindingFlags.Public)));
            }
        }
        public static bool GetRoleConfigPrefix(MethodBase __originalMethod, ref RoleConfig __result)
        {
            __result = RoleConfigs[__originalMethod];
            return false;
        }
    }
}
