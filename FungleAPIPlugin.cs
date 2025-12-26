using AmongUs.GameOptions;
using BepInEx;
using BepInEx.Configuration;
using BepInEx.Core.Logging.Interpolation;
using BepInEx.Logging;
using BepInEx.Unity.IL2CPP;
using BepInEx.Unity.IL2CPP.Utils;
using Epic.OnlineServices;
using FungleAPI.Components;
using FungleAPI.Configuration;
using FungleAPI.Configuration.Attributes;
using FungleAPI.Configuration.Patches;
using FungleAPI.Cosmetics;
using FungleAPI.GameOver;
using FungleAPI.ModCompatibility;
using FungleAPI.Patches;
using FungleAPI.PluginLoading;
using FungleAPI.Role;
using FungleAPI.Translation;
using FungleAPI.Utilities;
using FungleAPI.Utilities.Assets;
using FungleAPI.Utilities.Prefabs;
using HarmonyLib;
using Hazel;
using Il2CppInterop.Runtime;
using Il2CppInterop.Runtime.Injection;
using Il2CppSystem.Text;
using InnerNet;
using Microsoft.VisualBasic;
using Steamworks;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using TMPro;
using Unity.Services.Core.Internal;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;
using static FungleAPI.PluginLoading.ModPlugin;
using static Il2CppSystem.Linq.Expressions.Interpreter.NullableMethodCallInstruction;

namespace FungleAPI
{
	[BepInProcess("Among Us.exe")]
	[BepInPlugin(ModId, "FungleAPI", ModV)]
	public class FungleAPIPlugin : BasePlugin
	{
        public const string ModId = "com.rafael.fungleapi";
        public const string ModV = "0.2.6";
        public static Harmony Harmony = new Harmony(ModId);
        public static FungleAPIPlugin Instance;
		public override void Load()
		{
            Instance = this;
            if (Plugin == null)
            {
                Log.LogError("Failed creating ModPlugin from API");
            }
            Harmony.PatchAll();
            IL2CPPChainloader.Instance.PluginLoad += new Action<PluginInfo, Assembly, BasePlugin>(delegate (PluginInfo pluginInfo, Assembly assembly, BasePlugin basePlugin)
            {
                IFungleBasePlugin fungleBasePlugin = basePlugin as IFungleBasePlugin;
                if (fungleBasePlugin != null)
                {
                    ModPluginManager.RegisterMod(basePlugin, fungleBasePlugin.ModVersion, new Action(fungleBasePlugin.LoadAssets), fungleBasePlugin.ModName, fungleBasePlugin.ModCredits);
                    fungleBasePlugin.OnRegisterInFungleAPI();
                }
            });
            SceneManager.add_sceneLoaded(new Action<Scene, LoadSceneMode>(delegate (Scene scene, LoadSceneMode _)
            {
                if (!loaddedAssets)
                {
                    CosmeticManager.SetPaletta();
                    loadAssets();
                    loaddedAssets = true;
                    MCIActive = MCIUtils.GetMCI() != null;
                }
                if (scene.name == "MainMenu" && !rolesRegistered)
                {
                    Plugin.Roles = RoleManager.Instance.DontDestroy().AllRoles.ToArray().Concat(Plugin.Roles).ToList();
                    foreach (KeyValuePair<Type, RoleTypes> pair in CustomRoleManager.RolesToRegister)
                    {
                        RoleManager.Instance.AllRoles.Add(CustomRoleManager.Register(pair.Key, ModPluginManager.GetModPlugin(pair.Key.Assembly), pair.Value));
                    }
                    rolesRegistered = true;
                }
            }));
            Log.LogInfo("Thanks MiraAPI for some features, if you like this API consider using MiraAPI as well :)");
            SetErrorMessages();
        }
        internal static bool MCIActive;
        private static bool rolesRegistered;
        internal static bool loaddedAssets;
        internal static ModPlugin plugin;
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
                    plugin.ModCredits = "[" + plugin.RealName + " v" + plugin.ModVersion + "]";
                    plugin.LocalMod = new ModPlugin.Mod(plugin);
                    plugin.PluginPreset = new Configuration.Presets.PluginPreset() { Plugin = plugin, CurrentPresetVersion = Instance.Config.Bind("Presets", "Current Version", ConfigurationManager.NullId) };
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
        internal static Action loadAssets = new Action(FungleAssets.LoadAll);
        internal static void SetErrorMessages()
        {
            Translator errorMessage = new Translator("Failed to sync mods with the host. Please make sure you have the same mods and versions installed as the host.");
            errorMessage.AddTranslation(SupportedLangs.Latam, "Error al sincronizar los mods con el anfitrión. Asegúrate de tener los mismos mods y versiones que el anfitrión.");
            errorMessage.AddTranslation(SupportedLangs.Brazilian, "Houve uma falha ao sincronizar os mods com o host. Certifique-se de ter os mesmos mods e versões instaladas que o host.");
            errorMessage.AddTranslation(SupportedLangs.Portuguese, "Houve uma falha ao sincronizar os mods com o host. Certifique-se de ter os mesmos mods e versões instalados que o host.");
            errorMessage.AddTranslation(SupportedLangs.Korean, "호스트와 모드를 동기화하지 못했습니다. 동일한 모드와 버전을 설치했는지 확인하세요.");
            errorMessage.AddTranslation(SupportedLangs.Russian, "Не удалось синхронизировать моды с хостом. Убедитесь, что у вас установлены те же моды и версии, что и у хоста.");
            errorMessage.AddTranslation(SupportedLangs.Dutch, "Kan mods niet synchroniseren met de host. Zorg ervoor dat je dezelfde mods en versies hebt geïnstalleerd als de host.");
            errorMessage.AddTranslation(SupportedLangs.Filipino, "Nabigong i-sync ang mga mod sa host. Siguraduhing pareho ang mods at bersyon na naka-install gaya ng sa host.");
            errorMessage.AddTranslation(SupportedLangs.French, "Échec de la synchronisation des mods avec l’hôte. Veuillez vérifier que vous avez les mêmes mods et versions que l’hôte.");
            errorMessage.AddTranslation(SupportedLangs.German, "Mods konnten nicht mit dem Host synchronisiert werden. Bitte stellen Sie sicher, dass Sie dieselben Mods und Versionen wie der Host installiert haben.");
            errorMessage.AddTranslation(SupportedLangs.Italian, "Impossibile sincronizzare le mod con l'host. Assicurati di avere le stesse mod e versioni installate dell'host.");
            errorMessage.AddTranslation(SupportedLangs.Japanese, "ホストとMODを同期できませんでした。ホストと同じMODとバージョンがインストールされているか確認してください。");
            errorMessage.AddTranslation(SupportedLangs.Spanish, "Error al sincronizar los mods con el anfitrión. Asegúrate de tener los mismos mods y versiones que el anfitrión.");
            errorMessage.AddTranslation(SupportedLangs.SChinese, "无法与主机同步模组。请确保您安装了与主机相同的模组和版本。");
            errorMessage.AddTranslation(SupportedLangs.TChinese, "無法與主機同步模組。請確認您安裝的模組與主機相同且版本一致。");
            errorMessage.AddTranslation(SupportedLangs.Irish, "Theip ar shioncronú na mods leis an óstach. Cinntigh go bhfuil na mods agus na leaganacha céanna agat leis an óstach.");
            Translator errorMessage2 = new Translator("Missing mods were found on your client. Please make sure you have the same mods and versions installed as the host.");
            errorMessage2.AddTranslation(SupportedLangs.Latam, "Se encontraron mods faltantes en tu cliente. Asegúrate de tener los mismos mods y versiones instalados que el anfitrión.");
            errorMessage2.AddTranslation(SupportedLangs.Brazilian, "Foram encontrados mods faltando no seu cliente. Certifique-se de ter os mesmos mods e versões instalados que o host.");
            errorMessage2.AddTranslation(SupportedLangs.Portuguese, "Foram encontrados mods faltando no seu cliente. Certifique-se de ter os mesmos mods e versões instalados que o host.");
            errorMessage2.AddTranslation(SupportedLangs.Korean, "클라이언트에서 누락된 모드가 발견되었습니다. 호스트와 동일한 모드와 버전이 설치되어 있는지 확인하세요.");
            errorMessage2.AddTranslation(SupportedLangs.Russian, "В вашем клиенте обнаружены отсутствующие моды. Пожалуйста, убедитесь, что у вас установлены те же моды и версии, что и у хоста.");
            errorMessage2.AddTranslation(SupportedLangs.Dutch, "Er zijn ontbrekende mods gevonden in je client. Zorg ervoor dat je dezelfde mods en versies hebt geïnstalleerd als de host.");
            errorMessage2.AddTranslation(SupportedLangs.Filipino, "May mga nawawalang mod na natagpuan sa iyong client. Siguraduhing naka-install ang parehong mga mod at bersyon tulad ng host.");
            errorMessage2.AddTranslation(SupportedLangs.French, "Des mods manquants ont été détectés sur votre client. Veuillez vérifier que vous avez les mêmes mods et versions installés que l'hôte.");
            errorMessage2.AddTranslation(SupportedLangs.German, "In Ihrem Client wurden fehlende Mods gefunden. Bitte stellen Sie sicher, dass Sie dieselben Mods und Versionen installiert haben wie der Host.");
            errorMessage2.AddTranslation(SupportedLangs.Italian, "Sono stati trovati mod mancanti nel tuo client. Assicurati di avere installati gli stessi mod e le stesse versioni dell'host.");
            errorMessage2.AddTranslation(SupportedLangs.Japanese, "クライアントに不足しているMODが見つかりました。ホストと同じMODとバージョンがインストールされているか確認してください。");
            errorMessage2.AddTranslation(SupportedLangs.Spanish, "Se encontraron mods faltantes en tu cliente. Asegúrate de tener los mismos mods y versiones instalados que el anfitrión.");
            errorMessage2.AddTranslation(SupportedLangs.SChinese, "在您的客户端中发现缺失的模组。请确保已安装与主机相同的模组和版本。");
            errorMessage2.AddTranslation(SupportedLangs.TChinese, "在您的客戶端中發現缺少的模組。請確保已安裝與主機相同的模組和版本。");
            errorMessage2.AddTranslation(SupportedLangs.Irish, "Fuarthas modanna ar iarraidh i do chliant. Déan cinnte go bhfuil na modanna agus na leaganacha céanna agat leis an óstach.");
            Translator errorMessage3 = new Translator("Missing mods were found on the host's client. Please make sure you have the same mods and versions installed as the host.");
            errorMessage3.AddTranslation(SupportedLangs.Latam, "Se encontraron mods faltantes en el cliente del anfitrión. Asegúrate de tener los mismos mods y versiones instalados que el anfitrión.");
            errorMessage3.AddTranslation(SupportedLangs.Brazilian, "Foram encontrados mods faltando no cliente do host. Certifique-se de ter os mesmos mods e versões instalados que o host.");
            errorMessage3.AddTranslation(SupportedLangs.Portuguese, "Foram encontrados mods faltando no cliente do host. Certifique-se de ter os mesmos mods e versões instalados que o host.");
            errorMessage3.AddTranslation(SupportedLangs.Korean, "호스트 클라이언트에서 누락된 모드가 발견되었습니다. 동일한 모드와 버전이 설치되어 있는지 확인하세요.");
            errorMessage3.AddTranslation(SupportedLangs.Russian, "На клиенте хоста обнаружены отсутствующие моды. Убедитесь, что у вас установлены те же моды и версии, что и у хоста.");
            errorMessage3.AddTranslation(SupportedLangs.Dutch, "Er zijn ontbrekende mods gevonden op de client van de host. Zorg ervoor dat je dezelfde mods en versies hebt geïnstalleerd als de host.");
            errorMessage3.AddTranslation(SupportedLangs.Filipino, "May mga nawawalang mod na natagpuan sa client ng host. Siguraduhing naka-install ang parehong mga mod at bersyon gaya ng sa host.");
            errorMessage3.AddTranslation(SupportedLangs.French, "Des mods manquants ont été détectés sur le client de l’hôte. Veuillez vérifier que vous avez les mêmes mods et versions installés que l’hôte.");
            errorMessage3.AddTranslation(SupportedLangs.German, "Auf dem Client des Hosts wurden fehlende Mods gefunden. Bitte stellen Sie sicher, dass Sie dieselben Mods und Versionen installiert haben wie der Host.");
            errorMessage3.AddTranslation(SupportedLangs.Italian, "Sono stati trovati mod mancanti nel client dell'host. Assicurati di avere installati gli stessi mod e le stesse versioni dell'host.");
            errorMessage3.AddTranslation(SupportedLangs.Japanese, "ホストのクライアントで不足しているMODが見つかりました。ホストと同じMODとバージョンがインストールされているか確認してください。");
            errorMessage3.AddTranslation(SupportedLangs.Spanish, "Se encontraron mods faltantes en el cliente del anfitrión. Asegúrate de tener los mismos mods y versiones instalados que el anfitrión.");
            errorMessage3.AddTranslation(SupportedLangs.SChinese, "在主机的客户端中发现缺失的模组。请确保您已安装与主机相同的模组和版本。");
            errorMessage3.AddTranslation(SupportedLangs.TChinese, "在主機的客戶端中發現缺少的模組。請確保您已安裝與主機相同的模組和版本。");
            errorMessage3.AddTranslation(SupportedLangs.Irish, "Fuarthas modanna ar iarraidh i gcliant an óstaigh. Déan cinnte go bhfuil na modanna agus na leaganacha céanna agat leis an óstach.");
            Translator errorMessage4 = new Translator("Some of your mods are different from the host's. Please make sure you have the same mods and versions installed as the host.");
            errorMessage4.AddTranslation(SupportedLangs.Latam, "Algunos de tus mods son diferentes a los del anfitrión. Asegúrate de tener los mismos mods y versiones instalados que el anfitrión.");
            errorMessage4.AddTranslation(SupportedLangs.Brazilian, "Alguns de seus mods são diferentes dos do host. Certifique-se de ter os mesmos mods e versões instalados que o host.");
            errorMessage4.AddTranslation(SupportedLangs.Portuguese, "Alguns dos seus mods são diferentes dos do host. Certifique-se de ter os mesmos mods e versões instalados que o host.");
            errorMessage4.AddTranslation(SupportedLangs.Korean, "일부 모드가 호스트의 것과 다릅니다. 동일한 모드와 버전이 설치되어 있는지 확인하세요.");
            errorMessage4.AddTranslation(SupportedLangs.Russian, "Некоторые из ваших модов отличаются от модов хоста. Убедитесь, что у вас установлены те же моды и версии, что и у хоста.");
            errorMessage4.AddTranslation(SupportedLangs.Dutch, "Sommige van je mods verschillen van die van de host. Zorg ervoor dat je dezelfde mods en versies hebt geïnstalleerd als de host.");
            errorMessage4.AddTranslation(SupportedLangs.Filipino, "Ilan sa iyong mga mod ay iba sa nasa host. Siguraduhing naka-install ang parehong mga mod at bersyon tulad ng sa host.");
            errorMessage4.AddTranslation(SupportedLangs.French, "Certains de vos mods sont différents de ceux de l’hôte. Veuillez vérifier que vous avez les mêmes mods et versions installés que l’hôte.");
            errorMessage4.AddTranslation(SupportedLangs.German, "Einige Ihrer Mods unterscheiden sich von denen des Hosts. Bitte stellen Sie sicher, dass Sie dieselben Mods und Versionen installiert haben wie der Host.");
            errorMessage4.AddTranslation(SupportedLangs.Italian, "Alcuni dei tuoi mod sono diversi da quelli dell'host. Assicurati di avere installati gli stessi mod e le stesse versioni dell'host.");
            errorMessage4.AddTranslation(SupportedLangs.Japanese, "一部のMODがホストのものと異なります。ホストと同じMODとバージョンがインストールされているか確認してください。");
            errorMessage4.AddTranslation(SupportedLangs.Spanish, "Algunos de tus mods son diferentes a los del anfitrión. Asegúrate de tener los mismos mods y versiones instalados que el anfitrión.");
            errorMessage4.AddTranslation(SupportedLangs.SChinese, "您的部分模组与主机的不一致。请确保您已安装与主机相同的模组和版本。");
            errorMessage4.AddTranslation(SupportedLangs.TChinese, "您的部分模組與主機的不一致。請確保您已安裝與主機相同的模組和版本。");
            errorMessage4.AddTranslation(SupportedLangs.Irish, "Tá roinnt de do mhodanna difriúil ó mhodanna an óstaigh. Cinntigh go bhfuil na modanna agus na leaganacha céanna agat leis an óstach.");
            DisconnectPopup.ErrorMessages.Add(AmongUsClientPatch.FailedToSyncOptionsError, errorMessage.StringName);
            DisconnectPopup.ErrorMessages.Add(AmongUsClientPatch.MissingMods, errorMessage2.StringName);
            DisconnectPopup.ErrorMessages.Add(AmongUsClientPatch.MissingModsOnHost, errorMessage3.StringName);
            DisconnectPopup.ErrorMessages.Add(AmongUsClientPatch.NotTheSameMods, errorMessage4.StringName);
        }
	}
}
