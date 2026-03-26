using AmongUs.GameOptions;
using BepInEx;
using BepInEx.Configuration;
using BepInEx.Core.Logging.Interpolation;
using BepInEx.Logging;
using BepInEx.Unity.IL2CPP;
using BepInEx.Unity.IL2CPP.Utils;
using BepInEx.Unity.IL2CPP.Utils.Collections;
using Epic.OnlineServices;
using FungleAPI.Components;
using FungleAPI.Configuration;
using FungleAPI.Configuration.Attributes;
using FungleAPI.Configuration.Patches;
using FungleAPI.Cosmetics;
using FungleAPI.Cosmetics.Patches;
using FungleAPI.Event;
using FungleAPI.Event.API;
using FungleAPI.GameOver;
using FungleAPI.ModCompatibility;
using FungleAPI.Networking;
using FungleAPI.Patches;
using FungleAPI.Player;
using FungleAPI.PluginLoading;
using FungleAPI.Role;
using FungleAPI.Translation;
using FungleAPI.Utilities;
using FungleAPI.Utilities.Assets;
using FungleAPI.Utilities.Prefabs;
using HarmonyLib;
using Hazel;
using Il2CppInterop.Generator.Extensions;
using Il2CppInterop.Runtime;
using Il2CppInterop.Runtime.Injection;
using Il2CppSystem.Text;
using InnerNet;
using Microsoft.VisualBasic;
using Steamworks;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using TMPro;
using Unity.Services.Core.Internal;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;
using static BepInEx.BepInDependency;

[assembly: AssemblyMetadata("Reactor.ModFlags", "RequireOnAllClients")]

namespace FungleAPI
{
    /// <summary>
    /// The API BasePlugin class
    /// </summary>
	[BepInProcess("Among Us.exe")]
	[BepInPlugin(ModId, "FungleAPI", ModV)]
    [BepInDependency("gg.reactor.api", DependencyFlags.SoftDependency)]
    public class FungleAPIPlugin : BasePlugin
	{
        public const string ModId = "io.github.obit508.fungleapi";
        public const string ModV = "0.2.8";
        public static Harmony Harmony = new Harmony(ModId);
        public static FungleAPIPlugin Instance;
        internal static FungleHelper Helper;
        internal static ModPlugin plugin;
        private static bool rolesRegistered;
        /// <summary>
        /// The API Plugin
        /// </summary>
        public static ModPlugin Plugin
        {
            get
            {
                if (plugin == null)
                {
                    plugin = new ModPlugin();
                    ModPluginManager.Register(plugin, Instance);
                    plugin.ModName = "Vanilla";
                    plugin.ModVersion = ModV;
                    plugin.ModCredits = $"[{plugin.RealName} v{plugin.ModVersion}]";
                    plugin.PageLink = "https://github.com/OBit508/FungleAPI";
                    plugin.LocalMod = new ModPlugin.Mod(plugin);
                    plugin.PluginPreset = new Configuration.Presets.PluginPreset()
                    {
                        Plugin = plugin,
                        CurrentPresetVersion = Instance.Config.Bind("Presets", "Current Version", ConfigurationManager.NullId)
                    };
                    if (plugin.PluginPreset.CurrentPresetVersion.Value == ConfigurationManager.NullId)
                    {
                        plugin.PluginPreset.CurrentPresetVersion.Value = ConfigurationManager.CurrentVersion;
                    }
                    plugin.PluginPreset.Initialize();
                    ModPlugin.AllPlugins.Add(plugin);
                }
                return plugin;
            }
        }
        public override void Load()
        {
            Instance = this;

            // Cria o ModPlugin da API
            if (Plugin == null)
            {
                Log.LogError("Failed creating ModPlugin from API");
            }
            Harmony.PatchAll();

            // Da patch nos InnerNetObjects para o sistema de Rpc
            CustomRpcManager.PatchInnerNetObjects();
            IL2CPPChainloader.Instance.PluginLoad += (pluginInfo, assembly, basePlugin) => // Chamado quando um mod é carregado
            {
                // Se o mod tiver a interface de registro da API ele auto registra e não precisa chamar direto no ModPluginManager
                if (basePlugin is IFungleBasePlugin fungle)
                {
                    ModPluginManager.RegisterMod(basePlugin, fungle.ModVersion, fungle.LoadAssets, fungle.ModName, fungle.ModCredits);
                    fungle.OnRegisterInFungleAPI();
                }
            };
            IL2CPPChainloader.Instance.Finished += () => // Chamado quando o BeplnEx termina de carregar os mods
            {
                // Organiza os mods registrados por GUID
                List<ModPlugin> ordered = ModPlugin.AllPlugins.OrderBy(p => p.LocalMod.GUID, StringComparer.Ordinal).ToList();
                ordered.Remove(Plugin);
                ModPlugin.AllPlugins.Clear();
                ModPlugin.AllPlugins.Add(Plugin);
                ModPlugin.AllPlugins.AddRange(ordered);
                foreach (ModPlugin mod in ModPlugin.AllPlugins)
                {
                    ModPluginManager.RegisterTypes(mod);
                }

                // Inicializa o suporte para alguns mods
                ReactorSupport.Initialize();
                LevelImpostorSupport.Initialize();
                SubmergedSupport.Initialize();

                // Carrega as cores (futuramente mais cosméticos)
                CosmeticManager.SetPaletta();
            };
            SceneManager.add_sceneLoaded(new Action<Scene, LoadSceneMode>(delegate (Scene scene, LoadSceneMode loadSceneMode)
            {
                // Carrega os arquivos assim que o jogo realmente abre
                if (!Helpers.GameIsRunning)
                {
                    Helpers.GameIsRunning = true;
                    Type type = typeof(Constants);
                    HarmonyLib.Patches patches = Harmony.GetPatchInfo(type.GetMethod("GetBroadcastVersion"));
                    HarmonyLib.Patches patches2 = Harmony.GetPatchInfo(type.GetMethod("IsVersionModded"));
                    if (patches.Prefixes.Any(p => p.PatchMethod.DeclaringType.Assembly != Plugin.ModAssembly) ||
                        patches.Postfixes.Any(p => p.PatchMethod.DeclaringType.Assembly != Plugin.ModAssembly) ||
                        patches2.Prefixes.Any(p => p.PatchMethod.DeclaringType.Assembly != Plugin.ModAssembly) ||
                        patches2.Postfixes.Any(p => p.PatchMethod.DeclaringType.Assembly != Plugin.ModAssembly))
                    {
                        CustomRpcManager.SafeModEnabled = true;
                    }
                    ResourceHelper.CallLoadAssets();
                }

                // Registra as funções
                if (scene.name == "MainMenu" && !rolesRegistered)
                {
                    Plugin.Roles = RoleManager.Instance.DontDestroy().AllRoles.ToArray().Concat(Plugin.Roles).ToList();
                    CustomRoleManager.AllRoles.AddRange(RoleManager.Instance.AllRoles.ToSystemList());
                    foreach (KeyValuePair<Type, RoleTypes> pair in CustomRoleManager.RolesToRegister)
                    {
                        RoleManager.Instance.AllRoles.Add(CustomRoleManager.Register(pair.Key, ModPluginManager.GetModPlugin(pair.Key.Assembly), pair.Value));
                    }
                    EventManager.CallEvent(new RegisterRoles());
                    rolesRegistered = true;
                }
            }));
            Log.LogInfo("Thanks MiraAPI for some features, if you like this API consider using MiraAPI as well :)");

            // Carrega as mensagens de erro do handshake
            DisconnectPopup.ErrorMessages.Add((DisconnectReasons)244, FungleTranslation.FailedToSyncSettings);

            // Adiciona um MonoBehaviour no BasePlugin para alguns metodos do Helpers
            Helper = AddComponent<FungleHelper>();
        }
        internal class FungleHelper : MonoBehaviour { }
    }
}
