using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace FungleAPI.PluginLoading
{
    public struct BepInMod
    {
        public static Dictionary<Assembly, BepInMod> Mods = new Dictionary<Assembly, BepInMod>();
        public string GUID;
        public string Version;
        public string Name;
        public Assembly Assembly;
        public BepInMod(string guid, string version, string name, Assembly assembly)
        {
            GUID = guid;
            Version = version;
            Name = name;
            Assembly = assembly;;
            Mods.Add(assembly, this);
        }
        public static BepInMod GetMod(Assembly assembly)
        {
            if (Mods.TryGetValue(assembly, out BepInMod bepInMod))
            {
                return bepInMod;
            }
            return default;
        }
    }
}
