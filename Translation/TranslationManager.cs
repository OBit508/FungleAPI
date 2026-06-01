using FungleAPI.Assets;
using Il2CppSystem.CodeDom;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace FungleAPI.Translation
{
    /// <summary>
    /// Manage the translation system
    /// </summary>
    public static class TranslationManager
    {
        private static Dictionary<string, Translator> Strings = new Dictionary<string, Translator>();
        public static readonly Dictionary<string, SupportedLangs> LanguageCodes = new Dictionary<string, SupportedLangs>()
        {
            { "en", SupportedLangs.English },
            { "es-419", SupportedLangs.Latam },
            { "pt-br", SupportedLangs.Brazilian },
            { "pt-pt", SupportedLangs.Portuguese },
            { "ko", SupportedLangs.Korean },
            { "ru", SupportedLangs.Russian },
            { "nl", SupportedLangs.Dutch },
            { "fil", SupportedLangs.Filipino },
            { "fr", SupportedLangs.French },
            { "de", SupportedLangs.German },
            { "it", SupportedLangs.Italian },
            { "ja", SupportedLangs.Japanese },
            { "es", SupportedLangs.Spanish },
            { "zh-cn", SupportedLangs.SChinese },
            { "zh-hans", SupportedLangs.SChinese },
            { "zh-tw", SupportedLangs.TChinese },
            { "zh-hk", SupportedLangs.TChinese },
            { "zh-hant", SupportedLangs.TChinese },
            { "ga", SupportedLangs.Irish }
        };
        /// <summary>
        /// Stores translators mapped by StringNames keys
        /// </summary>
        public static Dictionary<StringNames, Translator> Translators = new Dictionary<StringNames, Translator>();
        /// <summary>
        /// Stores translators mapped by TranslationHelper attribute ids
        /// </summary>
        public static Dictionary<string, Translator> TranslationIDs = new Dictionary<string, Translator>();
        /// <summary>
        /// Default translator used when no translation is found
        /// </summary>
        public static Translator None = new Translator("STRMISS");
        internal static int validId = int.MinValue;
        /// <summary>
        /// Returns a equivalent Translator for the given string
        /// </summary>
        public static Translator GetTranslator(string str)
        {
            if (Strings.TryGetValue(str, out Translator translator))
            {
                return translator;
            }
            Translator tr = new Translator(str);
            Strings[str] = tr;
            return tr;
        }
        /// <summary>
        /// Returns a equivalent StringNames for the given string
        /// </summary>
        public static StringNames GetStringName(string str)
        {
            return GetTranslator(str).StringName;
        }
        public static void TranslateFromJsonFolder<T>(Assembly assembly, string folder) where T : class
        {
            TranslateFromJsonFolder(assembly, typeof(T), folder);
        }
        public static void TranslateFromJsonFolder(Assembly assembly, Type type, string folder)
        {
            Dictionary<string, Translator> waiting = new Dictionary<string, Translator>();

            foreach (PropertyInfo propertyInfo in type.GetProperties(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic))
            {
                if (propertyInfo.GetValue(null) is Translator translator)
                {
                    waiting.Add(propertyInfo.Name, translator);
                }
            }

            foreach (string str in assembly.GetManifestResourceNames().Where(s => s.StartsWith(folder) && s.EndsWith(".json")))
            {
                JsonElement json = JsonDocument.Parse(AssetLoader.ReadText(assembly, str)).RootElement;

                if (json.TryGetProperty("language", out JsonElement language))
                {
                    if (LanguageCodes.TryGetValue(language.GetString(), out SupportedLangs supportedLangs))
                    {
                        foreach (KeyValuePair<string, Translator> translator in waiting)
                        {
                            if (json.TryGetProperty(translator.Key, out JsonElement value))
                            {
                                translator.Value.AddTranslation(supportedLangs, value.GetString());
                            }
                        }
                    }
                }
            }
        }
    }
}
