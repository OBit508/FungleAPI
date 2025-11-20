using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FungleAPI.Translation
{
    public class Translator
    {
        public Translator(string defaultText)
        {
            Default = defaultText;
            StringName = (StringNames)validId;
            validId--;
            All.Add(this);
        }
        internal static int validId = -99999;
        internal static List<Translator> All = new List<Translator>();
        internal string Default;
        public StringNames StringName;
        internal Dictionary<SupportedLangs, string> Strings = new Dictionary<SupportedLangs, string>();
        public static Translator None = new Translator("STRMISS");
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
