using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FungleAPI.Translation
{
    public static class TranslationManager
    {
        public static Dictionary<StringNames, Translator> Translators = new Dictionary<StringNames, Translator>();
        public static Dictionary<string, Translator> TranslationIDs = new Dictionary<string, Translator>();
        public static Translator None = new Translator("STRMISS");
        internal static int validId = int.MinValue;
    }
}
