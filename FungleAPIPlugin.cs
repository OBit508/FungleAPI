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
        private static GameObject CreditScreen;
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
        public void ClickOnModName()
        {
            if (CreditScreen == null && AccountManager.InstanceExists)
            {
                CreditScreen = GameObject.Instantiate(AccountManager.Instance.signInScreen.gameObject, AccountManager.Instance.signInScreen.transform.parent);
                CreditScreen.GetComponent<SignInScreen>().Destroy();
                CreditScreen.GetComponent<PauseTimeoutTimer>().Destroy();
                void SetText(TextMeshPro text, string str)
                {
                    text.GetComponent<TextTranslatorTMP>()?.Destroy();
                    text.GetComponent<PlatformTextTranslationTMP>()?.Destroy();
                    text.text = str;
                }
                SetText(CreditScreen.transform.GetChild(2).GetComponent<TextMeshPro>(), "FungleAPI Credits");
                SetText(CreditScreen.transform.GetChild(3).GetComponent<TextMeshPro>(), "FungleAPI is a lightweight modding API developed primarily by a single contributor," +
                    " with support from a small group of collaborators for testing and development." +
                    "\n\nIts design is influenced by existing community projects such as MiraAPI and Reactor," +
                    " incorporating selected ideas and approaches that helped shape its architecture.");
                PassiveButton closeButton = CreditScreen.transform.GetChild(5).GetComponent<PassiveButton>();
                TransitionOpen transitionOpen = CreditScreen.GetComponent<TransitionOpen>();
                transitionOpen.OnClose.RemoveAllListeners();
                transitionOpen.OnClose.AddListener(new Action(() => CreditScreen.gameObject.SetActive(false)));
                closeButton.SetNewAction(transitionOpen.Close);
                TextTranslatorTMP textTranslator = closeButton.transform.GetChild(1).GetComponent<TextTranslatorTMP>();
                textTranslator.TargetText = StringNames.Close;
                textTranslator.ResetText();
                PassiveButton github = CreditScreen.transform.GetChild(6).GetComponent<PassiveButton>();
                github.SetNewAction(() => Application.OpenURL("https://github.com/OBit508/FungleAPI"));
                SetText(github.transform.GetChild(1).GetComponent<TextMeshPro>(), "GitHub");
                CreditScreen.transform.GetChild(4).gameObject.Destroy();
            }
            CreditScreen?.SetActive(true);
        }
        internal class FungleHelper : MonoBehaviour { public void OnApplicationQuit() { OptionManager.SaveOptionCollections(); } }
    }
}
