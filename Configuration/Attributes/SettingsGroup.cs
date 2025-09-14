using FungleAPI.Translation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using static Il2CppSystem.Globalization.CultureInfo;

namespace FungleAPI.Configuration.Attributes
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class SettingsGroup : Attribute
    {
        public SettingsGroup(string name)
        {
            groupName = new Translator(name);
        }
        public Translator groupName;
        public string Id;
        public List<ModdedOption> Options = new List<ModdedOption>();
        public StringNames GroupName => groupName.StringName;
        public void Initialize(PropertyInfo property, object obj)
        {
            if (property.PropertyType == typeof(string))
            {
                Id = (string)property.GetValue(obj);
            }
        }
    }
}
