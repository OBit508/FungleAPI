using AmongUs.GameOptions;
using BepInEx;
using BepInEx.Unity.IL2CPP;
using FungleAPI.Assets;
using FungleAPI.Cosmetics;
using FungleAPI.Cosmetics.Helpers;
using FungleAPI.Event;
using FungleAPI.Event.Api;
using FungleAPI.Event.BelpInEx;
using FungleAPI.Extensions;
using FungleAPI.GameOptions;
using FungleAPI.GameOptions.Lobby;
using FungleAPI.ModCompatibility;
using FungleAPI.ModCompatibility.ReactorSupportTemp;
using FungleAPI.Networking;
using FungleAPI.PluginLoading;
using FungleAPI.Ship;
using FungleAPI.Translation;
using FungleAPI.Utilities;
using HarmonyLib;
using Il2CppInterop.Runtime.Injection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using TMPro;
using UnityEngine;
using UnityEngine.AddressableAssets.ResourceLocators;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.SceneManagement;
using static BepInEx.BepInDependency;

[assembly: AssemblyMetadata("Reactor.ModFlags", "RequireOnAllClients, DisableServerAuthority")]

namespace FungleAPI
{
    /// <summary>
    /// The API BasePlugin class
    /// </summary>
	[BepInProcess("Among Us.exe")]
	[BepInPlugin(ModId, "FungleAPI", ModV)]
    [BepInDependency("gg.reactor.api", DependencyFlags.SoftDependency)]
    [BepInDependency("Submerged", DependencyFlags.SoftDependency)]
    public class FungleApiPlugin : BasePlugin, IFungleBasePlugin
	{
        public const string ModId = "io.github.obit508.fungleapi";
        public const string ModV = "0.2.8";
        public static readonly Harmony Harmony = new Harmony(ModId);
        public static FungleApiPlugin Instance { get; private set; }

        public string ModName { get; } = "Vanilla";
        public string ModVersion { get; } = ModV;

        internal static FungleHelper Helper;
        /// <summary>
        /// The API Plugin
        /// </summary>
        public static ModPlugin Plugin => FunglePlugin<FungleApiPlugin>.Plugin;
        public override void Load()
        {
            ModPlugin plugin = new ModPlugin();
            ModPluginManager.Register(plugin, Assembly.GetExecutingAssembly(), this);
            plugin.FunglePlugin = this;
            plugin.RulePreset = Config.Bind("Essential", "RulePreset", (byte)RulesPresets.Standard);
            plugin.LobbyTabs = new List<LobbyTab>() { new VanillaSettingsTab() { Plugin = plugin }, new TeamTab() { Plugin = plugin }, new RoleTab() { Plugin = plugin } };
            ModPluginManager.AllPlugins.Add(plugin);
            ModPluginManager.AllAssemblies.Add(plugin.ModAssembly, plugin);

            Instance = this;

            ClassInjector.RegisterTypeInIl2Cpp<CosmeticLocator>(new RegisterTypeOptions
            {
                Interfaces = new Il2CppInterfaceCollection([typeof(IResourceLocator)])
            });
            ClassInjector.RegisterTypeInIl2Cpp<CosmeticProvider>(new RegisterTypeOptions
            {
                Interfaces = new Il2CppInterfaceCollection([typeof(IResourceProvider)])
            });

            Harmony.PatchAll();

            // Da patch nos InnerNetObjects para o sistema de Rpc
            CustomRpcManager.PatchInnerNetObjects();

            ReactorCompatibility.CheckReactor();
            SubmergedCompatibility.CheckSubmerged();

            IL2CPPChainloader.Instance.PluginLoad += (pluginInfo, assembly, basePlugin) => // Chamado quando um mod é carregado
            {
                // Se o mod tiver a interface de registro da API ele auto registra e não precisa chamar direto no ModPluginManager
                if (basePlugin is IFungleBasePlugin fungle)
                {
                    ModPlugin plugin = new ModPlugin();
                    ModPluginManager.Register(plugin, assembly, basePlugin);
                    plugin.FunglePlugin = basePlugin as IFungleBasePlugin;
                    plugin.RulePreset = basePlugin.Config.Bind("Essential", "RulePreset", (byte)RulesPresets.Standard);
                    ModPluginManager.AllPlugins.Add(plugin);
                    ModPluginManager.AllAssemblies.Add(plugin.ModAssembly, plugin);
                    fungle.AlmostLoaded();
                }
            };
            IL2CPPChainloader.Instance.Finished += () => // Chamado quando o BeplnEx termina de carregar os mods
            {
                foreach (PluginInfo pluginInfo in IL2CPPChainloader.Instance.Plugins.Values)
                {
                    Assembly assembly = pluginInfo.Instance.GetType().Assembly;

                    BepInMod bepInMod = new BepInMod(pluginInfo.Metadata.GUID, pluginInfo.Metadata.Version.Clean(), pluginInfo.Metadata.Name, assembly);

                    ModPlugin modPlugin = ModPluginManager.GetModPlugin(assembly);
                    if (modPlugin != null)
                    {
                        modPlugin.LocalMod = bepInMod;
                        HandShakeManager.RequiredMods.Add(pluginInfo.Metadata.GUID, bepInMod);

                        if (modPlugin.FunglePlugin.ApperOnCredits)
                        {
                            ReactorCompatibility.Instance?.Register(modPlugin.FunglePlugin.ModName, modPlugin.FunglePlugin.ModVersion, false, (l) => l == ReactorCreditsLocation.PingTracker);
                        }
                    }
                }

                // Organiza os mods registrados por GUID

                IOrderedEnumerable<ModPlugin> ordered = ModPluginManager.AllPlugins.FindAll(p => p != Plugin).OrderBy(p => p.LocalMod.GUID, StringComparer.Ordinal);
                ModPluginManager.AllPlugins.Clear();
                ModPluginManager.AllPlugins.Add(Plugin);
                ModPluginManager.AllPlugins.AddRange(ordered);

                foreach (ModPlugin mod in ModPluginManager.AllPlugins)
                {
                    ModPluginManager.RegisterTypes(mod);
                }

                EventManager.CallEvent(new FinishedPluginLoadingEvent());
            };
            SceneManager.add_sceneLoaded(new Action<Scene, LoadSceneMode>(delegate (Scene scene, LoadSceneMode loadSceneMode)
            {
                if (!Helpers.GameIsRunning)
                {
                    Helpers.GameIsRunning = true;

                    EventManager.CallEvent(new FirstSceneLoadEvent());

                    FungleAssets.LoadAll();
                }
            }));

            // Adiciona um MonoBehaviour no BasePlugin para alguns metodos do Helpers
            Helper = AddComponent<FungleHelper>();
        }
        public void ShowCreditsScreen()
        {
            DisconnectPopup.Instance.ShowCustom("Thanks for using Fungle Api!!!");
            Application.OpenURL("https://github.com/OBit508/FungleAPI");
        }
        public System.Collections.IEnumerator CoLoadAssets(TextMeshPro loadingText)
        {
            string baseText = $"<font=\"Brook SDF\" material=\"Brook SDF - WhiteOutline\">{FungleTranslation.LoadingShipPrefabsText.GetString()}";
            yield return ShipPrefabLoader.CoLoadShipPrefabs(loadingText, baseText);

            ChangeableValue<bool> func = new ChangeableValue<bool>(false);
            System.Collections.IEnumerator CoLoadAssets()
            {
                foreach (Func<object> obj in AssetLoader.LateAssets)
                {
                    try
                    {
                        obj?.Invoke();
                    }
                    catch { }
                    yield return null;
                }
                AssetLoader.LateAssets = null;
                func.Value = true;
            }
            Helpers.StartCoroutine(CoLoadAssets());

            yield return CoAnimateDots(loadingText, $"<font=\"Brook SDF\" material=\"Brook SDF - WhiteOutline\">{FungleTranslation.LoadingAssetsText.GetString()}", func);
        }
        internal static System.Collections.IEnumerator CoAnimateDots(TextMeshPro textMeshPro, string baseText, ChangeableValue<bool> func)
        {
            int dots = 0;
            float timer = 0f;
            while (!func.Value)
            {
                timer += Time.deltaTime;
                if (timer >= 0.35f)
                {
                    timer = 0f;
                    dots = (dots % 3) + 1;
                    textMeshPro.text = baseText + new string('.', dots) + "</font>";
                }
                yield return null;
            }
        }
        internal class FungleHelper : MonoBehaviour { public void OnApplicationQuit() { OptionManager.SaveOptionCollections(); } }
    }
}
