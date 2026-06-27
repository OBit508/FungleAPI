using AmongUs.GameOptions;
using BepInEx;
using BepInEx.Unity.IL2CPP;
using FungleAPI.Api;
using FungleAPI.Attributes;
using FungleAPI.Base.Rpc;
using FungleAPI.Components;
using FungleAPI.Cosmetics;
using FungleAPI.Event;
using FungleAPI.Freeplay;
using FungleAPI.GameOptions;
using FungleAPI.GameOver;
using FungleAPI.GModes;
using FungleAPI.Hud;
using FungleAPI.Networking;
using FungleAPI.Player.Patches;
using FungleAPI.Role;
using FungleAPI.Ship.Patches;
using FungleAPI.Teams;
using FungleAPI.Translation;
using FungleAPI.Utilities;
using HarmonyLib;
using Il2CppInterop.Runtime;
using Il2CppInterop.Runtime.Injection;
using Il2CppSystem.Text.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Metadata.Ecma335;

namespace FungleAPI.PluginLoading
{
    /// <summary>
    /// A class that helps the mod system to work
    /// </summary>
    public static class ModPluginManager
    {
        public static readonly Dictionary<Assembly, ModPlugin> AllAssemblies = new Dictionary<Assembly, ModPlugin>();
        public static readonly List<ModPlugin> AllPlugins = new List<ModPlugin>();
        internal static void Register(ModPlugin plugin, Assembly assembly, BasePlugin basePlugin)
        {
            plugin.ModAssembly = assembly;
            plugin.BasePlugin = basePlugin;
            HashSet<Type> visited = new HashSet<Type>();
            foreach (Type type in plugin.ModAssembly.GetTypes())
            {
                AddTypeRecursively(type, plugin, visited);
            }

            plugin.AllPriorityTypes = plugin.AllPriorityTypes.OrderBy(x => x.Key).ToDictionary(x => x.Key, x => x.Value);

            plugin.ImplementedCredits = basePlugin.GetType().GetMethod(nameof(IFungleBasePlugin.ShowCreditsScreen), BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.DeclaredOnly) != null;

            EventManager.RegisterEvents(plugin);
        }
        private static void AddTypeRecursively(Type type, ModPlugin plugin, HashSet<Type> visited)
        {
            if (!visited.Add(type)) return;

            plugin.AllTypes.Add(type);

            RegisterPriority registerPriority = type.GetCustomAttribute<RegisterPriority>();
            if (registerPriority != null)
            {
                plugin.AllPriorityTypes.Add(registerPriority.Priority, type);
            }

            foreach (Type nested in type.GetNestedTypes(AccessTools.all))
            {
                AddTypeRecursively(nested, plugin, visited);
            }
        }
        public static void RegisterTypes(ModPlugin plugin)
        {
            plugin.BasePlugin.Log.LogInfo($"Registering Types on FungleAPI");

            IFungleBasePlugin fungleBasePlugin = plugin.FunglePlugin;

            if (fungleBasePlugin.UseAutoRegistration)
            {
                foreach (Type type in plugin.AllPriorityTypes.Values)
                {
                    if (type.ShouldIgnore()) continue;
                    try
                    {
                        bool registeredInIl2cpp = false;
                        ProcessType(type, plugin, ref registeredInIl2cpp, plugin.Settings != null, plugin.FolderConfig != null, plugin.Cosmetics != null);
                        if (!registeredInIl2cpp && type.GetCustomAttribute<RegisterTypeInIl2Cpp>() != null)
                        {
                            ClassInjector.RegisterTypeInIl2Cpp(type);
                        }
                    }
                    catch (Exception ex)
                    {
                        plugin.BasePlugin.Log.LogError($"Failed to register type {type.FullName}: {ex}");
                    }
                }

                foreach (Type type in plugin.AllTypes.FindAll(t => !plugin.AllPriorityTypes.Values.Contains(t)))
                {
                    if (type.ShouldIgnore()) continue;
                    try
                    {
                        bool registeredInIl2cpp = false;
                        ProcessType(type, plugin, ref registeredInIl2cpp, plugin.Settings != null, plugin.FolderConfig != null, plugin.Cosmetics != null);
                        if (!registeredInIl2cpp && type.GetCustomAttribute<RegisterTypeInIl2Cpp>() != null)
                        {
                            ClassInjector.RegisterTypeInIl2Cpp(type);
                        }

                        TranslationAttribute translationAttribute = type.GetCustomAttribute<TranslationAttribute>();
                        if (translationAttribute != null)
                        {
                            TranslationManager.TranslateFromJsonFolder(plugin.ModAssembly, type, translationAttribute.JsonFolderPath);
                        }
                    }
                    catch (Exception ex)
                    {
                        plugin.BasePlugin.Log.LogError($"Failed to register type {type.FullName}: {ex}");
                    }
                }
            }

            if (plugin != FungleApiPlugin.Plugin)
            {
                fungleBasePlugin?.LoadTabs(plugin);
            }
            if (plugin.Settings == null)
            {
                plugin.Settings = new RoomSettings();
            }
            if (plugin.FolderConfig == null)
            {
                plugin.FolderConfig = new ModFolderConfig();
            }
            if (plugin.Cosmetics == null)
            {
                plugin.Cosmetics = new ModCosmetics();
            }
            fungleBasePlugin?.FullyLoaded();
        }
        private static void ProcessType(Type type, ModPlugin plugin, ref bool registeredInIl2cpp, bool hasSettings, bool hasFolderConfig, bool hasCosmetics)
        {
            if (!hasSettings && typeof(RoomSettings).IsAssignableFrom(type))
            {
                plugin.Settings = (RoomSettings)Activator.CreateInstance(type);
                return;
            }
            else if (!hasFolderConfig && typeof(ModFolderConfig).IsAssignableFrom(type))
            {
                plugin.FolderConfig = (ModFolderConfig)Activator.CreateInstance(type);
                return;
            }
            else if (!hasCosmetics && typeof(ModCosmetics).IsAssignableFrom(type))
            {
                plugin.Cosmetics = CosmeticManager.RegisterCosmetics(type, plugin);
                return;
            }
            else if (typeof(CustomAbilityButton).IsAssignableFrom(type))
            {
                HudHelper.RegisterButton(type, plugin);
                return;
            }
            else if (typeof(RoleBehaviour).IsAssignableFrom(type) && typeof(ICustomRole).IsAssignableFrom(type))
            {
                plugin.HasRoles = true;
                CustomRoleManager.RegisterRole(type, plugin);
                return;
            }
            else if (typeof(BaseGameOver).IsAssignableFrom(type))
            {
                GameOverManager.RegisterGameOver(type, plugin);
                return;
            }
            else if (typeof(BaseGameMode).IsAssignableFrom(type))
            {
                GameModeManager.RegisterGameMode(type, plugin);
                return;
            }
            else if (typeof(ModdedTeam).IsAssignableFrom(type))
            {
                ModdedTeamManager.RegisterTeam(type, plugin);
                return;
            }
            else if (typeof(RpcHelper).IsAssignableFrom(type))
            {
                CustomRpcManager.RegisterRpc(type, plugin);
                return;
            }
            else if (typeof(PlayerComponent).IsAssignableFrom(type))
            {
                registeredInIl2cpp = true;
                ClassInjector.RegisterTypeInIl2Cpp(type);
                PlayerControlPatch.AllPlayerComponents.Add(Il2CppType.From(type));
                return;
            }
            else if (typeof(DeadBodyComponent).IsAssignableFrom(type))
            {
                registeredInIl2cpp = true;
                ClassInjector.RegisterTypeInIl2Cpp(type);
                DeadBodyHelper.AllBodyComponents.Add(Il2CppType.From(type));
                return;
            }
            else if (typeof(VentComponent).IsAssignableFrom(type))
            {
                registeredInIl2cpp = true;
                ClassInjector.RegisterTypeInIl2Cpp(type);
                VentPatch.AllVentComponents.Add(Il2CppType.From(type));
                return;
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
    }
}
