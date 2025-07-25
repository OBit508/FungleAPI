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

namespace FungleAPI.Role.Configuration
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class BoolConfig : CustomConfig
    {
        public BoolConfig(StringNames configName)
        {
            ConfigName = configName;
        }
        public override void Initialize(Type type, PropertyInfo property, object obj)
        {
            if (property.PropertyType == typeof(bool))
            {
                ModPlugin plugin = ModPlugin.GetModPlugin(type.Assembly);
                bool value = (bool)property.GetValue(obj);
                localValue = plugin.BasePlugin.Config.Bind(plugin.ModName + " - " + type.FullName, ConfigName.GetString(), value.ToString());
                onlineValue = value.ToString();
            }
        }
    }
}
