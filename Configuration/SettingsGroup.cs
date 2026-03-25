using Epic.OnlineServices;
using FungleAPI.Attributes;
using FungleAPI.Configuration.Attributes;
using FungleAPI.PluginLoading;
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
    /// <summary>
    /// Group with options (used in ModSettings)
    /// </summary>
    public class SettingsGroup
    {
        public List<ModdedOption> Options;
        public Translator GroupName;
        public virtual void Initialize(ModPlugin modPlugin)
        {
            Type type = GetType();
            Options = ConfigurationManager.RegisterAllOptions(type, ConfigFileType.GameOptions, modPlugin);
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
