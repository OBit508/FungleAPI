using Epic.OnlineServices;
using FungleAPI.Configuration.Helpers;
using FungleAPI.Translation;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using static Il2CppSystem.Globalization.CultureInfo;

namespace FungleAPI.Configuration.Attributes
{
    public class SettingsGroup
    {
        public List<ModdedOption> Options = new List<ModdedOption>();
        public StringNames GroupName => groupName.StringName;
        public virtual void Initialize()
        {
            Type type = GetType();
            foreach (PropertyInfo property in type.GetProperties())
            {
                ModdedOption att = (ModdedOption)property.GetCustomAttribute(typeof(ModdedOption));
                if (att != null)
                {
                    att.Initialize(property);
                    MethodInfo method = property.GetGetMethod(true);
                    if (method != null)
                    {
                        HarmonyHelper.Patches.Add(method, new Func<object>(att.GetReturnedValue));
                        FungleAPIPlugin.Harmony.Patch(method, new HarmonyMethod(typeof(HarmonyHelper).GetMethod("GetPrefix", BindingFlags.Static | BindingFlags.Public)));
                    }
                    Options.Add(att);
                }
            }
            groupName = new Translator(type.Name);
        }
        private Translator groupName;
    }
}
