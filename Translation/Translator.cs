using System.Collections.Generic;

namespace FungleAPI.Translation
{
    public class Translator
    {
        public Translator(string defaultText)
        {
            Default = defaultText;
            StringName = (StringNames)TranslationManager.validId;
            TranslationManager.validId++;
            TranslationManager.Translators.Add(StringName, this);
        }
        public string Default;
        public StringNames StringName;
        public Dictionary<SupportedLangs, string> Strings = new Dictionary<SupportedLangs, string>();
        public Translator AddTranslation(SupportedLangs lang, string text)
        {
            if (!Strings.ContainsKey(lang))
            {
                Strings.Add(lang, text);
            }
            return this;
        }
        public string GetString()
        {
            foreach (KeyValuePair<SupportedLangs, string> pair in Strings)
            {
                if (pair.Key == TranslationController.Instance.currentLanguage.languageID)
                {
                    return pair.Value;
                }
            }
            return Default;
        }
        public override string ToString()
        {
            return GetString();
        }
    }
}
