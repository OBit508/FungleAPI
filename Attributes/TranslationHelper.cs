using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FungleAPI.Attributes
{
    public class TranslationHelper : Attribute
    {
        public string TranslationID;
        public TranslationHelper(string translationID)
        {
            TranslationID = translationID;
        }
    }
}
