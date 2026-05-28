using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BepInEx
{
    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
    [Conditional("CodeGeneration")]
    internal sealed class BepInAutoPluginAttribute : Attribute
    {
        public BepInAutoPluginAttribute(string id = null, string name = null, string version = null)
        {
        }
    }
}
