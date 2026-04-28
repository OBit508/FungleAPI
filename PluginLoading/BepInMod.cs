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
        private static Dictionary<BepInMod, Assembly> assemblys = new Dictionary<BepInMod, Assembly>();
        public static Dictionary<Assembly, BepInMod> Mods = new Dictionary<Assembly, BepInMod>();
        public string GUID;
        public string Version;
        public string Name;
        public Assembly Assembly
        {
            get
            {
                if (assemblys.TryGetValue(this, out Assembly assembly))
                {
                    return assembly;
                }
                return FungleAPIPlugin.Plugin.ModAssembly;
            }
        }
        public BepInMod(string guid, string version, string name, Assembly assembly)
        {
            GUID = guid;
            Version = version;
            Name = name;
            assemblys.Add(this, assembly);
            Mods.Add(assembly, this);
        }
        public override bool Equals(object obj)
        {
            if (obj is BepInMod other)
            {
                return GUID == other.GUID;
            }
            return false;
        }
        public override int GetHashCode()
        {
            return GUID?.GetHashCode() ?? 0;
        }
        public override string ToString()
        {
            return GUID;
        }
        public static BepInMod GetMod(Assembly assembly)
        {
            if (Mods.TryGetValue(assembly, out BepInMod bepInMod))
            {
                return bepInMod;
            }
            return new BepInMod() { GUID = FungleAPIPlugin.ModId, Version = FungleAPIPlugin.ModV, Name = "FungleAPI" };
        }
    }
}
