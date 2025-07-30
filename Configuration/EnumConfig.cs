using BepInEx.Configuration;
using FungleAPI.Patches;
using FungleAPI.Roles;
using FungleAPI.Utilities;
using Il2CppSystem.Web;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using static Il2CppSystem.Linq.Expressions.Interpreter.CastInstruction.CastInstructionNoT;

namespace FungleAPI.Configuration
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class EnumConfig : CustomConfig
    {
        public EnumConfig(string configName, string[] defaultValue)
        {
            ConfigName = configName;
            Enum = defaultValue;
        }
        public override void Initialize(Type type, PropertyInfo property, object obj)
        {
            if (property.PropertyType == typeof(string))
            {
                ModPlugin plugin = ModPlugin.GetModPlugin(type.Assembly);
                string value = (string)property.GetValue(obj);
                localValue = plugin.BasePlugin.Config.Bind(plugin.ModName + " - " + type.FullName, ConfigName, value.ToString());
                onlineValue = value.ToString();
                FullConfigName = plugin.ModName + type.FullName + property.Name + value.GetType().FullName;
            }
        }
        internal int currentIndex;
        internal string[] Enum;
        public void BackValue()
        {
            if (currentIndex - 1 <= 0)
            {
                currentIndex = Enum.Count() - 1;
            }
            else
            {
                currentIndex--;
            }
            SetValue(Enum[currentIndex]);
        }
        public void NextValue()
        {
            if (currentIndex + 1 >= Enum.Count())
            {
                currentIndex = 0;
            }
            else
            {
                currentIndex++;
            }
            SetValue(Enum[currentIndex]);
        }
    }
}
