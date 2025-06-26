using BepInEx.Configuration;
using BepInEx.Unity.IL2CPP;
using FungleAPI.Role.Teams;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace FungleAPI.LoadMod
{
    public class ModPlugin
    {
        public static ModPlugin GetModPlugin(Assembly assembly)
        {
            foreach (ModPlugin mod in AllPlugins)
            {
                if (mod.ModAssembly == assembly)
                {
                    return mod;
                }
            }
            return null;
        }
        public static ModPlugin RegisterMod(BasePlugin basePlugin, string ModName = null)
        {
            ModPlugin plugin = new ModPlugin();
            if (FungleAPIPlugin.Plugin != null)
            {
                plugin.ModAssembly = basePlugin.GetType().Assembly;
                plugin.ModName = plugin.ModAssembly.GetName().Name;
                if (ModName != null)
                {
                    plugin.ModName = ModName;
                }
                plugin.BasePlugin = basePlugin;
                AllPlugins.Add(plugin);
            }
            return plugin;
        }
        public static List<ModPlugin> AllPlugins = new List<ModPlugin>();
        internal ModPlugin()
        {
        }
        public ConfigEntry<T> CreateConfig<T>(string Name, T value)
        {
            return BasePlugin.Config.Bind<T>(ModName + " - Configs", Name, value);
        }
        public string ModName;
        public Assembly ModAssembly;
        public BasePlugin BasePlugin;
        public List<RoleBehaviour> Roles = new List<RoleBehaviour>();
        public List<ModdedTeam> Teams = new List<ModdedTeam>();
        internal int rpcId;
    }
}
