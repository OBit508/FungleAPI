using AmongUs.GameOptions;
using BepInEx.Configuration;
using BepInEx.Unity.IL2CPP;
using FungleAPI;
using FungleAPI.Role;
using FungleAPI.Role.Teams;
using FungleAPI.Roles;
using FungleAPI.Rpc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.AddressableAssets;
using xCloud;

namespace FungleAPI
{
    public class ModPlugin
    {
        internal ModPlugin()
        {
        }
        internal static ModPlugin Register(BasePlugin basePlugin)
        {
            ModPlugin plugin = new ModPlugin();
            plugin.ModAssembly = basePlugin.GetType().Assembly;
            plugin.ModName = plugin.ModAssembly.GetName().Name;
            plugin.BasePlugin = basePlugin;
            foreach (Type type in plugin.ModAssembly.GetTypes())
            {
                if (typeof(CustomAbilityButton).IsAssignableFrom(type) && type != typeof(CustomAbilityButton))
                {
                    CustomAbilityButton.RegisterButton(type, plugin);
                }
                else if (typeof(RoleBehaviour).IsAssignableFrom(type) && typeof(ICustomRole).IsAssignableFrom(type))
                {
                    CustomRoleManager.RegisterRole(type, plugin);
                }
                else if (typeof(ModdedTeam).IsAssignableFrom(type) && type != typeof(ModdedTeam))
                {
                    ModdedTeam.RegisterTeam(type, plugin);
                }
                else if (typeof(RpcHelper).IsAssignableFrom(type) && type != typeof(RpcHelper) && type != typeof(CustomRpc<>))
                {
                    CustomRpcManager.RegisterRpc(type, plugin);
                }
            }
            AllPlugins.Add(plugin);
            return plugin;
        }
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
                plugin = Register(basePlugin);
                if (ModName != null)
                {
                    plugin.ModName = ModName;
                }
            }
            return plugin;
        }
        public ConfigEntry<T> CreateConfig<T>(string Name, T value)
        {
            return BasePlugin.Config.Bind(ModName + " - Configs", Name, value);
        }
        public string ModName;
        public Assembly ModAssembly;
        public BasePlugin BasePlugin;
        public List<RoleBehaviour> Roles = new List<RoleBehaviour>();
        public static List<ModPlugin> AllPlugins = new List<ModPlugin>();
    }
}
