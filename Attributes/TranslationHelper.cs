using FungleAPI.Translation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FungleAPI.Attributes
{
    /// <summary>
    /// This attribute allows the class or property to which it has been assigned to be translated based on an ID added in the TranslationManager
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Property, AllowMultiple = false)]
    public class TranslationHelper : Attribute
    {
        public string TranslationID;
        public TranslationHelper(string translationID)
        {
            TranslationID = translationID;
        }
    }
}
