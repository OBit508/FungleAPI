using FungleAPI.Roles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using BepInEx.Configuration;
using FungleAPI.Patches;
using System.Reflection;
using FungleAPI.Utilities;

namespace FungleAPI.Configuration
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class BoolConfig : CustomConfig
    {
        public BoolConfig(string configName)
        {
            ConfigName = configName;
        }
        public override void Initialize(Type type, PropertyInfo property, object obj)
        {
            if (property.PropertyType == typeof(bool))
            {
                ModPlugin plugin = ModPlugin.GetModPlugin(type.Assembly);
                bool value = (bool)property.GetValue(obj);
                localValue = plugin.BasePlugin.Config.Bind(plugin.ModName + " - " + type.FullName, ConfigName, value.ToString());
                onlineValue = value.ToString();
                FullConfigName = plugin.ModName + type.FullName + property.Name + value.GetType().FullName;
            }
        }
    }
}
