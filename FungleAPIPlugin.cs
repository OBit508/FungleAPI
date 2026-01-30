using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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
        public const string ModId = "io.github.obit508.fungleapi";
        public const string ModV = "0.2.8";
        public static Harmony Harmony = new Harmony(ModId);
        public static FungleAPIPlugin Instance;
        internal static FungleHelper Helper;
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
            IL2CPPChainloader.Instance.Finished += new Action(delegate
            {
                ReactorSupport.Initialize();
                LevelImpostorSupport.Initialize();
                CosmeticManager.SetPaletta();
            });
            SceneManager.add_sceneLoaded(new Action<Scene, LoadSceneMode>(delegate (Scene scene, LoadSceneMode _)
            {
                if (!loaddedAssets)
                {
                    loadAssets();
                    loaddedAssets = true;
                    Type type = typeof(Constants);
                    HarmonyLib.Patches patches = Harmony.GetPatchInfo(type.GetMethod("GetBroadcastVersion"));
                    HarmonyLib.Patches patches2 = Harmony.GetPatchInfo(type.GetMethod("IsVersionModded"));
                    if (patches.Prefixes.Any(p => p.PatchMethod.DeclaringType.Assembly != Plugin.ModAssembly) || patches.Postfixes.Any(p => p.PatchMethod.DeclaringType.Assembly != Plugin.ModAssembly) || patches2.Prefixes.Any(p => p.PatchMethod.DeclaringType.Assembly != Plugin.ModAssembly) || patches2.Postfixes.Any(p => p.PatchMethod.DeclaringType.Assembly != Plugin.ModAssembly))
                    {
                        CustomRpcManager.SafeModEnabled = true;
                    }
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
            Helper = AddComponent<FungleHelper>();
        }
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
            Translator errorMessage = new Translator("Missing mods were found on the host's client.\nMods: ");
            errorMessage.AddTranslation(SupportedLangs.Latam, "Se encontraron mods faltantes en el cliente del anfitrión.\nMods: ");
            errorMessage.AddTranslation(SupportedLangs.Brazilian, "Foram encontrados mods faltando no cliente do Anfitrião.\nMods: ");
            errorMessage.AddTranslation(SupportedLangs.Portuguese, "Foram encontrados mods faltando no cliente do Anfitrião.\nMods: ");
            errorMessage.AddTranslation(SupportedLangs.Korean, "호스트 클라이언트에서 누락된 모드가 발견되었습니다.\n모드: ");
            errorMessage.AddTranslation(SupportedLangs.Russian, "На клиенте хоста обнаружены отсутствующие моды.\nМоды: ");
            errorMessage.AddTranslation(SupportedLangs.Dutch, "Er zijn ontbrekende mods gevonden op de client van de host.\nMods: ");
            errorMessage.AddTranslation(SupportedLangs.Filipino, "May mga nawawalang mod na natagpuan sa client ng host.\nMods: ");
            errorMessage.AddTranslation(SupportedLangs.French, "Des mods manquants ont été détectés sur le client de l’hôte.\nMods : ");
            errorMessage.AddTranslation(SupportedLangs.German, "Auf dem Client des Hosts wurden fehlende Mods gefunden.\nMods: ");
            errorMessage.AddTranslation(SupportedLangs.Italian, "Sono stati trovati mod mancanti nel client dell'host.\nMod: ");
            errorMessage.AddTranslation(SupportedLangs.Japanese, "ホストのクライアントで不足しているMODが見つかりました。\nMOD: ");
            errorMessage.AddTranslation(SupportedLangs.Spanish, "Se encontraron mods faltantes en el cliente del anfitrión.\nMods: ");
            errorMessage.AddTranslation(SupportedLangs.SChinese, "在主机的客户端中发现缺失的模组。\n模组： ");
            errorMessage.AddTranslation(SupportedLangs.TChinese, "在主機的客戶端中發現缺少的模組。\n模組： ");
            errorMessage.AddTranslation(SupportedLangs.Irish, "Fuarthas modanna ar iarraidh i gcliant an óstaigh.\nMods: ");
            Translator errorMessage2 = new Translator("Missing mods were found on the local client.\nMods: ");
            errorMessage2.AddTranslation(SupportedLangs.Latam, "Se encontraron mods faltantes en el cliente local.\nMods: ");
            errorMessage2.AddTranslation(SupportedLangs.Brazilian, "Foram encontrados mods faltando no cliente local.\nMods: ");
            errorMessage2.AddTranslation(SupportedLangs.Portuguese, "Foram encontrados mods faltando no cliente local.\nMods: ");
            errorMessage2.AddTranslation(SupportedLangs.Korean, "로컬 클라이언트에서 누락된 모드가 발견되었습니다.\n모드: ");
            errorMessage2.AddTranslation(SupportedLangs.Russian, "В локальном клиенте обнаружены отсутствующие моды.\nМоды: ");
            errorMessage2.AddTranslation(SupportedLangs.Dutch, "Er zijn ontbrekende mods gevonden op de lokale client.\nMods: ");
            errorMessage2.AddTranslation(SupportedLangs.Filipino, "May mga nawawalang mod na natagpuan sa lokal na client.\nMods: ");
            errorMessage2.AddTranslation(SupportedLangs.French, "Des mods manquants ont été détectés sur le client local.\nMods : ");
            errorMessage2.AddTranslation(SupportedLangs.German, "Im lokalen Client wurden fehlende Mods gefunden.\nMods: ");
            errorMessage2.AddTranslation(SupportedLangs.Italian, "Sono stati trovati mod mancanti nel client locale.\nMod: ");
            errorMessage2.AddTranslation(SupportedLangs.Japanese, "ローカルクライアントで不足しているMODが見つかりました。\nMOD: ");
            errorMessage2.AddTranslation(SupportedLangs.Spanish, "Se encontraron mods faltantes en el cliente local.\nMods: ");
            errorMessage2.AddTranslation(SupportedLangs.SChinese, "在本地客户端中发现缺失的模组。\n模组： ");
            errorMessage2.AddTranslation(SupportedLangs.TChinese, "在本地客戶端中發現缺少的模組。\n模組： ");
            errorMessage2.AddTranslation(SupportedLangs.Irish, "Fuarthas modanna ar iarraidh sa chliant áitiúil.\nMods: ");
            Translator errorMessage3 = new Translator("The same mods were not found between the host's client and the local client.\nMissing mods on the local client: 0.\nMissing mods on the host's client: ");
            errorMessage3.AddTranslation(SupportedLangs.Latam, "No se encontraron los mismos mods entre el cliente del anfitrión y el cliente local.\nMods faltantes en el cliente local: 0.\nMods faltantes en el cliente del anfitrión: ");
            errorMessage3.AddTranslation(SupportedLangs.Brazilian, "Não foram encontrados os mesmos mods no cliente do Anfitrião quanto ao cliente local.\nMods faltando no cliente local: 0.\nMods faltando no cliente do anfitrião: ");
            errorMessage3.AddTranslation(SupportedLangs.Portuguese, "Não foram encontrados os mesmos mods no cliente do Anfitrião quanto ao cliente local.\nMods em falta no cliente local: 0.\nMods em falta no cliente do anfitrião: ");
            errorMessage3.AddTranslation(SupportedLangs.Korean, "호스트 클라이언트와 로컬 클라이언트 간에 동일한 모드를 찾을 수 없습니다.\n로컬 클라이언트에서 누락된 모드: 0.\n호스트 클라이언트에서 누락된 모드: ");
            errorMessage3.AddTranslation(SupportedLangs.Russian, "Одинаковые моды не были найдены между клиентом хоста и локальным клиентом.\nОтсутствующие моды в локальном клиенте: 0.\nОтсутствующие моды в клиенте хоста: ");
            errorMessage3.AddTranslation(SupportedLangs.Dutch, "Dezelfde mods zijn niet gevonden tussen de client van de host en de lokale client.\nOntbrekende mods op de lokale client: 0.\nOntbrekende mods op de client van de host: ");
            errorMessage3.AddTranslation(SupportedLangs.Filipino, "Hindi natagpuan ang parehong mga mod sa pagitan ng client ng host at lokal na client.\nNawawalang mods sa lokal na client: 0.\nNawawalang mods sa client ng host: ");
            errorMessage3.AddTranslation(SupportedLangs.French, "Les mêmes mods n'ont pas été trouvés entre le client de l’hôte et le client local.\nMods manquants sur le client local : 0.\nMods manquants sur le client de l’hôte : ");
            errorMessage3.AddTranslation(SupportedLangs.German, "Dieselben Mods wurden zwischen dem Host-Client und dem lokalen Client nicht gefunden.\nFehlende Mods im lokalen Client: 0.\nFehlende Mods im Host-Client: ");
            errorMessage3.AddTranslation(SupportedLangs.Italian, "Non sono stati trovati gli stessi mod tra il client dell'host e il client locale.\nMod mancanti nel client locale: 0.\nMod mancanti nel client dell'host: ");
            errorMessage3.AddTranslation(SupportedLangs.Japanese, "ホストのクライアントとローカルクライアント間で同じMODが見つかりませんでした。\nローカルクライアントで不足しているMOD: 0。\nホストのクライアントで不足しているMOD: ");
            errorMessage3.AddTranslation(SupportedLangs.Spanish, "No se encontraron los mismos mods entre el cliente del anfitrión y el cliente local.\nMods faltantes en el cliente local: 0.\nMods faltantes en el cliente del anfitrión: ");
            errorMessage3.AddTranslation(SupportedLangs.SChinese, "在主机客户端与本地客户端之间未找到相同的模组。\n本地客户端缺失的模组：0。\n主机客户端缺失的模组：");
            errorMessage3.AddTranslation(SupportedLangs.TChinese, "在主機客戶端與本地客戶端之間未找到相同的模組。\n本地客戶端缺少的模組：0。\n主機客戶端缺少的模組：");
            errorMessage3.AddTranslation(SupportedLangs.Irish, "Níor aimsíodh na modanna céanna idir cliant an óstaigh agus an cliant áitiúil.\nModanna ar iarraidh sa chliant áitiúil: 0.\nModanna ar iarraidh i gcliant an óstaigh: ");
            Translator errorMessage4 = new Translator("The host is not modded.");
            errorMessage4.AddTranslation(SupportedLangs.Latam, "El anfitrión no está modificado.");
            errorMessage4.AddTranslation(SupportedLangs.Brazilian, "O anfitrião não está modificado.");
            errorMessage4.AddTranslation(SupportedLangs.Portuguese, "O anfitrião não está modificado.");
            errorMessage4.AddTranslation(SupportedLangs.Korean, "호스트가 모드가 적용되지 않았습니다.");
            errorMessage4.AddTranslation(SupportedLangs.Russian, "Хост не модифицирован.");
            errorMessage4.AddTranslation(SupportedLangs.Dutch, "De host is niet gemodificeerd.");
            errorMessage4.AddTranslation(SupportedLangs.Filipino, "Hindi modded ang host.");
            errorMessage4.AddTranslation(SupportedLangs.French, "L’hôte n’est pas modifié.");
            errorMessage4.AddTranslation(SupportedLangs.German, "Der Host ist nicht modifiziert.");
            errorMessage4.AddTranslation(SupportedLangs.Italian, "L'host non è modificato.");
            errorMessage4.AddTranslation(SupportedLangs.Japanese, "ホストはMODが適用されていません。");
            errorMessage4.AddTranslation(SupportedLangs.Spanish, "El anfitrión no está modificado.");
            errorMessage4.AddTranslation(SupportedLangs.SChinese, "主机未进行模组修改。");
            errorMessage4.AddTranslation(SupportedLangs.TChinese, "主機未進行模組修改。");
            errorMessage4.AddTranslation(SupportedLangs.Irish, "Níl an t-óstach modhnaithe.");
            Translator errorMessage5 = new Translator("There was a failure while verifying the installed mods.");
            errorMessage5.AddTranslation(SupportedLangs.Latam, "Hubo un fallo al verificar los mods instalados.");
            errorMessage5.AddTranslation(SupportedLangs.Brazilian, "Houve uma falha na verificação dos mods instalados.");
            errorMessage5.AddTranslation(SupportedLangs.Portuguese, "Houve uma falha na verificação dos mods instalados.");
            errorMessage5.AddTranslation(SupportedLangs.Korean, "설치된 모드를 확인하는 중 오류가 발생했습니다.");
            errorMessage5.AddTranslation(SupportedLangs.Russian, "Произошла ошибка при проверке установленных модов.");
            errorMessage5.AddTranslation(SupportedLangs.Dutch, "Er is een fout opgetreden bij het controleren van de geïnstalleerde mods.");
            errorMessage5.AddTranslation(SupportedLangs.Filipino, "Nagkaroon ng error habang sinusuri ang mga naka-install na mod.");
            errorMessage5.AddTranslation(SupportedLangs.French, "Une erreur s’est produite lors de la vérification des mods installés.");
            errorMessage5.AddTranslation(SupportedLangs.German, "Beim Überprüfen der installierten Mods ist ein Fehler aufgetreten.");
            errorMessage5.AddTranslation(SupportedLangs.Italian, "Si è verificato un errore durante la verifica delle mod installate.");
            errorMessage5.AddTranslation(SupportedLangs.Japanese, "インストールされているMODの確認中にエラーが発生しました。");
            errorMessage5.AddTranslation(SupportedLangs.Spanish, "Ocurrió un error al verificar los mods instalados.");
            errorMessage5.AddTranslation(SupportedLangs.SChinese, "验证已安装的模组时发生错误。");
            errorMessage5.AddTranslation(SupportedLangs.TChinese, "驗證已安裝的模組時發生錯誤。");
            errorMessage5.AddTranslation(SupportedLangs.Irish, "Tharla earráid agus na modanna suiteáilte á bhfíorú.");
            Translator errorMessage6 = new Translator("There was a failure while trying to sync the settings.");
            errorMessage6.AddTranslation(SupportedLangs.Latam, "Hubo un fallo al intentar sincronizar la configuración.");
            errorMessage6.AddTranslation(SupportedLangs.Brazilian, "Houve uma falha tentando sincronizar as configurações.");
            errorMessage6.AddTranslation(SupportedLangs.Portuguese, "Houve uma falha ao tentar sincronizar as configurações.");
            errorMessage6.AddTranslation(SupportedLangs.Korean, "설정을 동기화하는 동안 오류가 발생했습니다.");
            errorMessage6.AddTranslation(SupportedLangs.Russian, "Произошла ошибка при попытке синхронизировать настройки.");
            errorMessage6.AddTranslation(SupportedLangs.Dutch, "Er is een fout opgetreden bij het synchroniseren van de instellingen.");
            errorMessage6.AddTranslation(SupportedLangs.Filipino, "Nagkaroon ng error habang sinusubukang i-sync ang mga setting.");
            errorMessage6.AddTranslation(SupportedLangs.French, "Une erreur s’est produite lors de la synchronisation des paramètres.");
            errorMessage6.AddTranslation(SupportedLangs.German, "Beim Synchronisieren der Einstellungen ist ein Fehler aufgetreten.");
            errorMessage6.AddTranslation(SupportedLangs.Italian, "Si è verificato un errore durante il tentativo di sincronizzare le impostazioni.");
            errorMessage6.AddTranslation(SupportedLangs.Japanese, "設定を同期しようとしてエラーが発生しました。");
            errorMessage6.AddTranslation(SupportedLangs.Spanish, "Ocurrió un error al intentar sincronizar la configuración.");
            errorMessage6.AddTranslation(SupportedLangs.SChinese, "尝试同步设置时发生错误。");
            errorMessage6.AddTranslation(SupportedLangs.TChinese, "嘗試同步設定時發生錯誤。");
            errorMessage6.AddTranslation(SupportedLangs.Irish, "Tharla earráid agus iarracht á déanamh na socruithe a shioncronú.");
            DisconnectPopup.ErrorMessages.Add(HandShakeManager.MissingModsOnHost, errorMessage.StringName);
            DisconnectPopup.ErrorMessages.Add(HandShakeManager.MissingMods, errorMessage2.StringName);
            DisconnectPopup.ErrorMessages.Add(HandShakeManager.NotSameMods, errorMessage3.StringName);
            DisconnectPopup.ErrorMessages.Add(HandShakeManager.HostIsNotModded, errorMessage4.StringName);
            DisconnectPopup.ErrorMessages.Add(HandShakeManager.FailedToVerifyMods, errorMessage5.StringName);
            DisconnectPopup.ErrorMessages.Add(HandShakeManager.FailedToSyncOptions, errorMessage6.StringName);
        }
        internal class FungleHelper : MonoBehaviour
        {
            public IEnumerator Play(IEnumerator enumerator)
            {
                try
                {
                    yield return enumerator;
                }
                finally { }
            }
        }
    }
}
