using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FungleAPI.Translation
{
    /// <summary>
    /// Manage the translation system
    /// </summary>
    public static class TranslationManager
    {
        private static Dictionary<string, Translator> Strings = new Dictionary<string, Translator>();
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
    }
}
