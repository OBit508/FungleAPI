using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BepInEx.Preloader.Core.Patching
{
    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
    [Conditional("CodeGeneration")]
    internal sealed class PatcherAutoPluginAttribute : Attribute
    {
        public PatcherAutoPluginAttribute(string id = null, string name = null, string version = null)
        {
        }
    }
}