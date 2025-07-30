using BepInEx.Configuration;
using FungleAPI.Patches;
using FungleAPI.Roles;
using FungleAPI.Utilities;
using Rewired.Utils.Classes.Data;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace FungleAPI.Configuration
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class NumConfig : CustomConfig
    {
        public NumConfig(string configName, float minValue, float maxValue, float reduceValue = 1, float increceValue = 1)
        {
            ReduceValue = reduceValue;
            IncreceValue = increceValue;
            ConfigName = configName;
        }
        public override void Initialize(Type type, PropertyInfo property, object obj)
        {
            if (property.PropertyType == typeof(float))
            {
                ModPlugin plugin = ModPlugin.GetModPlugin(type.Assembly);
                float value = (float)property.GetValue(obj);
                localValue = plugin.BasePlugin.Config.Bind(plugin.ModName + " - " + type.FullName, ConfigName, value.ToString());
                onlineValue = value.ToString();
                FullConfigName = plugin.ModName + type.FullName + property.Name + value.GetType().FullName;
            }
        }
        public float ReduceValue;
        public float IncreceValue;
        public float MinValue;
        public float MaxValue;
    }
}
