using Epic.OnlineServices;
using FungleAPI.Attributes;
using FungleAPI.Configuration.Attributes;
using FungleAPI.Translation;
using FungleAPI.Utilities;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using static Il2CppSystem.Globalization.CultureInfo;

namespace FungleAPI.Configuration
{
    public class SettingsGroup
    {
        public List<ModdedOption> Options = new List<ModdedOption>();
        public Translator GroupName;
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
            TranslationHelper attributeTranslationID = type.GetCustomAttribute<TranslationHelper>();
            if (attributeTranslationID != null && TranslationManager.TranslationIDs.TryGetValue(attributeTranslationID.TranslationID, out Translator translator))
            {
                GroupName = translator;
            }
            else
            {
                GroupName = new Translator(type.Name);
            }
        }
    }
}
