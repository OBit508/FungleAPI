using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FungleAPI.Attributes
{
    /// <summary>
    /// 
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Property, AllowMultiple = false)]
    public class TranslationHelper : Attribute
    {
        /// <summary>
        /// 
        /// </summary>
        public string TranslationID;
        /// <summary>
        /// 
        /// </summary>
        public TranslationHelper(string translationID)
        {
            TranslationID = translationID;
        }
    }
}
