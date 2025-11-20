using FungleAPI.Configuration.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace FungleAPI.Configuration.Helpers
{
    internal static class HarmonyHelper
    {
        public static Dictionary<MethodBase, Func<object>> Patches = new Dictionary<MethodBase, Func<object>>();
        public static bool GetPrefix(MethodBase __originalMethod, ref object __result)
        {
            __result = Patches[__originalMethod]();
            return false;
        }
    }
}
