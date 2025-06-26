using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FungleAPI.Translation
{
    public class Translator
    {
        public static Translator CreateOrGetTranslator(string defaultText)
        {
            foreach (Translator t in All)
            {
                if (t.Default == defaultText)
                {
                    return t;
                }
            }
            return new Translator(defaultText);
        }
        internal Translator(string defaultText)
        {
            Default = defaultText;
            StringName = (StringNames)1000000 + All.Count + 100;
            All.Add(this);
        }
        internal static List<Translator> All = new List<Translator>();
        internal string Default;
        public StringNames StringName;
        internal Dictionary<SupportedLangs, string> Strings = new Dictionary<SupportedLangs, string>();
        public static Translator None = CreateOrGetTranslator("STRMISS");
        public Translator AddTranslation(SupportedLangs lang, string text)
        {
            if (!Strings.Keys.Contains(lang))
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
