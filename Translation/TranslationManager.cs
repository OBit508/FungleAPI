using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FungleAPI.Translation
{
    /// <summary>
    /// 
    /// </summary>
    public static class TranslationManager
    {
        /// <summary>
        /// 
        /// </summary>
        public static Dictionary<StringNames, Translator> Translators = new Dictionary<StringNames, Translator>();
        /// <summary>
        /// 
        /// </summary>
        public static Dictionary<string, Translator> TranslationIDs = new Dictionary<string, Translator>();
        /// <summary>
        /// 
        /// </summary>
        public static Translator None = new Translator("STRMISS");
        internal static int validId = int.MinValue;
    }
}
