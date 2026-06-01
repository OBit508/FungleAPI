using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FungleAPI.Translation
{
    [AttributeUsage(AttributeTargets.Class)]
    public class TranslationAttribute : Attribute 
    {
        public string JsonFolderPath;
        public TranslationAttribute(string jsonFolderPath)
        {
            JsonFolderPath = jsonFolderPath;
        }
    }
}
