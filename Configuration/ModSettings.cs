using Epic.OnlineServices;
using FungleAPI.Configuration.Attributes;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace FungleAPI.Configuration
{
    public class ModSettings
    {
        public List<SettingsGroup> Groups = new List<SettingsGroup>();
        public bool initialized;
        public virtual void Initialize()
        {
            if (!initialized)
            {
                Type type = GetType();
                List<ModdedOption> configs = new List<ModdedOption>();
                foreach (PropertyInfo property in type.GetProperties())
                {
                    ModdedOption att = (ModdedOption)property.GetCustomAttribute(typeof(ModdedOption));
                    if (att != null)
                    {
                        att.Initialize(type, property, this);
                        MethodInfo method = property.GetGetMethod(true);
                        if (method != null)
                        {
                            ConfigurationManager.Configs.Add(method, att);
                            FungleAPIPlugin.Harmony.Patch(method, new HarmonyMethod(typeof(ConfigurationManager).GetMethod("GetPrefix", BindingFlags.Static | BindingFlags.Public, null, new Type[] { typeof(MethodBase), property.PropertyType.MakeByRefType() }, null)));
                        }
                        configs.Add(att);
                    }
                }
                InitializeGroups(configs);
                initialized = true;
            }
        }
        public virtual void InitializeGroups(List<ModdedOption> options)
        {
            Type type = GetType();
            foreach (PropertyInfo property in type.GetProperties())
            {
                SettingsGroup group = (SettingsGroup)property.GetCustomAttribute(typeof(SettingsGroup));
                if (group != null)
                {
                    group.Initialize(property, this);
                    foreach (ModdedOption option in options)
                    {
                        if (option.GroupId == group.Id)
                        {
                            group.Options.Add(option);
                        }
                    }
                    Groups.Add(group);
                }
            }
        }
    }
}
