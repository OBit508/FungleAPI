using Hazel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FungleAPI.Configuration.Helpers
{
    /// <summary>
    /// Helper class
    /// </summary>
    public abstract class ConfigHelper
    {
        public string Name;
        public abstract string Compact();
        public abstract void Decompact(string str, bool local);
    }
}
