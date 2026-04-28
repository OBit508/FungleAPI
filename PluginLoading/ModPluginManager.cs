using AmongUs.GameOptions;
using BepInEx;
using BepInEx.Unity.IL2CPP;
using FungleAPI.Attributes;
using FungleAPI.Base.Roles;
using FungleAPI.Base.Rpc;
using FungleAPI.Components;
using FungleAPI.Cosmetics;
using FungleAPI.Event;
using FungleAPI.Freeplay;
using FungleAPI.GameMode;
using FungleAPI.GameMode.Patches;
using FungleAPI.GameOptions;
using FungleAPI.GameOver;
using FungleAPI.Hud;
using FungleAPI.Networking;
using FungleAPI.Player.Patches;
using FungleAPI.Role;
using FungleAPI.Role.Utilities;
using FungleAPI.Teams;
using FungleAPI.Translation;
using FungleAPI.Utilities;
using FungleAPI.Utilities.Assets;
using HarmonyLib;
using Il2CppInterop.Runtime;
using Il2CppInterop.Runtime.Injection;
using Rewired.Utils.Classes.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UIElements.UIR;
using xCloud;

namespace FungleAPI.PluginLoading
{
    /// <summary>
    /// A class that helps the mod system to work
    /// </summary>
    public static class ModPluginManager
    {
        public static readonly Dictionary<Assembly, ModPlugin> AllAssemblies = new Dictionary<Assembly, ModPlugin>();
        public static readonly List<ModPlugin> AllPlugins = new List<ModPlugin>();
        internal static void Register(ModPlugin plugin, BasePlugin basePlugin)
        {
            plugin.ModAssembly = basePlugin.GetType().Assembly;
            plugin.ModName = plugin.ModAssembly.GetName().Name;
            plugin.RealName = plugin.ModName;
            plugin.BasePlugin = basePlugin;
            HashSet<Type> visited = new HashSet<Type>();
            foreach (Type type in plugin.ModAssembly.GetTypes())
            {
                AddTypeRecursively(type, plugin.AllTypes, visited);
            }
            EventManager.RegisterEvents(plugin);
        }
        private static void AddTypeRecursively(Type type, List<Type> allTypes, HashSet<Type> visited)
        {
            if (!visited.Add(type)) return;
            allTypes.Add(type);
            foreach (Type nested in type.GetNestedTypes(AccessTools.all))
            {
                AddTypeRecursively(nested, allTypes, visited);
            }
        }
        public static void RegisterTypes(ModPlugin plugin)
        {
            foreach (Type type in plugin.AllTypes)
            {
                if (type.GetCustomAttribute<FungleIgnore>() != null) continue;
                try
                {
                    ProcessType(type, plugin);
                }
                catch (Exception ex)
                {
                    plugin.BasePlugin.Log.LogError($"Failed to register type {type.FullName}: {ex}");
                }
            }
            if (plugin != FungleAPIPlugin.Plugin)
            {
                IFungleBasePlugin fungleBasePlugin = plugin.BasePlugin as IFungleBasePlugin;
                if (fungleBasePlugin != null)
                {
                    fungleBasePlugin.LoadTabs(plugin);
                }
            }
        }
        private static void ProcessType(Type type, ModPlugin plugin)
        {
            if (type.GetCustomAttribute<RegisterTypeInIl2Cpp>() != null)
            {
                ClassInjector.RegisterTypeInIl2Cpp(type);
            }
            if (typeof(ModSettings).IsAssignableFrom(type))
            {
                plugin.Settings = (ModSettings)Activator.CreateInstance(type);
            }
            else if (typeof(ModFolderConfig).IsAssignableFrom(type))
            {
                plugin.FolderConfig = (ModFolderConfig)Activator.CreateInstance(type);
            }
            else if (typeof(CustomAbilityButton).IsAssignableFrom(type))
            {
                HudHelper.RegisterButton(type, plugin);
            }
            else if (typeof(RoleBehaviour).IsAssignableFrom(type) && typeof(ICustomRole).IsAssignableFrom(type))
            {
                plugin.HasRoles = true;
                CustomRoleManager.RegisterRole(type, plugin);
            }
            else if (typeof(CustomGameOver).IsAssignableFrom(type))
            {
                GameOverManager.RegisterGameOver(type, plugin);
            }
            else if (typeof(ModdedTeam).IsAssignableFrom(type))
            {
                ModdedTeamManager.RegisterTeam(type, plugin);
            }
            else if (typeof(RpcHelper).IsAssignableFrom(type))
            {
                CustomRpcManager.RegisterRpc(type, plugin);
            }
            else if (typeof(PlayerComponent).IsAssignableFrom(type))
            {
                ClassInjector.RegisterTypeInIl2Cpp(type);
                PlayerControlPatch.AllPlayerComponents.Add(Il2CppType.From(type));
            }
            else if (typeof(DeadBodyComponent).IsAssignableFrom(type))
            {
                ClassInjector.RegisterTypeInIl2Cpp(type);
                DeadBodyHelper.AllBodyComponents.Add(Il2CppType.From(type));
            }
            else if (typeof(VentComponent).IsAssignableFrom(type))
            {
                ClassInjector.RegisterTypeInIl2Cpp(type);
                VentPatch.AllVentComponents.Add(Il2CppType.From(type));
            }
            else if (typeof(ModCosmetics).IsAssignableFrom(type))
            {
                plugin.Cosmetics = CosmeticManager.RegisterCosmetics(type, plugin);
            }
            else if (typeof(CustomGameMode).IsAssignableFrom(type))
            {
                GameModeManager.RegisterGameMode(type, plugin);
            }
        }
        public static ModPlugin GetModPlugin(Assembly assembly)
        {
            if (AllAssemblies.TryGetValue(assembly, out ModPlugin modPlugin))
            {
                return modPlugin;
            }
            return null;
        }
        public static PluginInfo TryGetPluginInfo(BasePlugin basePlugin)
        {
            PluginInfo pluginInfo = IL2CPPChainloader.Instance.Plugins.Values.FirstOrDefault(p => p.Instance == basePlugin);
            if (pluginInfo != null)
            {
                return pluginInfo;
            }
            basePlugin?.Log.LogError("Failed to get PluginInfo");
            return null;
        }
        internal static ModPlugin RegisterMod(BasePlugin basePlugin, string modVersion, string modName, string modCredits)
        {
            ModPlugin plugin = new ModPlugin();
            Register(plugin, basePlugin);

            plugin.RulePreset = basePlugin.Config.Bind("Essential", "RulePreset", (byte)RulesPresets.Standard);

            plugin.ModName = modName;
            plugin.RealName = modName;
            int count = AllPlugins.Count(p => p.RealName == plugin.RealName);
            if (count > 0)
            {
                plugin.ModName += $" ({count})";
            }
            plugin.ModVersion = modVersion;
            plugin.ModCredits = modCredits;
            AllPlugins.Add(plugin);
            AllAssemblies.Add(plugin.ModAssembly, plugin);
            return plugin;
        }
    }
}
