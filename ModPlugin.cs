using AmongUs.GameOptions;
using BepInEx.Configuration;
using BepInEx.Unity.IL2CPP;
using FungleAPI;
using FungleAPI.Attributes;
using FungleAPI.Components;
using FungleAPI.Networking;
using FungleAPI.Networking.RPCs;
using FungleAPI.Patches;
using FungleAPI.Role;
using FungleAPI.Role.Teams;
using FungleAPI.Roles;
using FungleAPI.Utilities.Assets;
using Il2CppInterop.Runtime;
using Il2CppInterop.Runtime.Injection;
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
        internal static void Register(ModPlugin plugin, BasePlugin basePlugin)
        {
            plugin.ModAssembly = basePlugin.GetType().Assembly;
            plugin.ModName = plugin.ModAssembly.GetName().Name;
            plugin.BasePlugin = basePlugin;
            foreach (Type type in plugin.ModAssembly.GetTypes())
            {
                try
                {
                    if (type.GetCustomAttribute<RegisterTypeInIl2Cpp>() != null)
                    {
                        ClassInjector.RegisterTypeInIl2Cpp(type);
                    }
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
                    else if (typeof(PlayerComponent).IsAssignableFrom(type) && type != typeof(PlayerComponent))
                    {
                        ClassInjector.RegisterTypeInIl2Cpp(type);
                        PlayerPatches.AllPlayerComponents.Add(Il2CppType.From(type));
                    }
                    else if (typeof(BodyComponent).IsAssignableFrom(type) && type != typeof(BodyComponent))
                    {
                        ClassInjector.RegisterTypeInIl2Cpp(type);
                        DeadBodyHelper.AllBodyComponents.Add(Il2CppType.From(type));
                    }
                    else if (typeof(VentComponent).IsAssignableFrom(type) && type != typeof(VentComponent))
                    {
                        ClassInjector.RegisterTypeInIl2Cpp(type);
                        VentPatch.AllVentComponents.Add(Il2CppType.From(type));
                    }
                }
                catch (Exception ex)
                {
                    basePlugin.Log.LogError(ex);
                }
            }
            AllPlugins.Add(plugin);
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
        public static ModPlugin RegisterMod(BasePlugin basePlugin, Action loadAssets = null, string ModName = null)
        {
            ModPlugin plugin = new ModPlugin();
            if (FungleAPIPlugin.Plugin != null)
            {
                Register(plugin, basePlugin);
                if (ModName != null)
                {
                    plugin.ModName = ModName;
                }
            }
            if (loadAssets != null)
            {
                FungleAPIPlugin.loadAssets += loadAssets;
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
        public List<ModdedTeam> Teams = new List<ModdedTeam>();
        public static List<ModPlugin> AllPlugins = new List<ModPlugin>();
    }
}
