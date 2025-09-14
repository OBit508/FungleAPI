using AmongUs.GameOptions;
using BepInEx;
using BepInEx.Configuration;
using BepInEx.Core.Logging.Interpolation;
using BepInEx.Logging;
using BepInEx.Unity.IL2CPP;
using BepInEx.Unity.IL2CPP.Utils;
using FungleAPI.Configuration;
using FungleAPI.Components;
using FungleAPI.Role;
using FungleAPI.Role.Patches;
using FungleAPI.Role.Teams;
using FungleAPI.Networking;
using FungleAPI.Translation;
using FungleAPI.Utilities;
using FungleAPI.Utilities.Assets;
using HarmonyLib;
using Hazel;
using Il2CppInterop.Runtime.Injection;
using Il2CppSystem.Text;
using InnerNet;
using Steamworks;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using Unity.Services.Core.Internal;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;
using FungleAPI.Patches;

namespace FungleAPI
{
	[BepInProcess("Among Us.exe")]
	[BepInPlugin(ModId, "FungleAPI", ModV)]
	public class FungleAPIPlugin : BasePlugin
	{
        public const string ModId = "com.rafael.fungleapi";
        public const string ModV = "0.1.6";
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
            SceneManager.add_sceneLoaded(new Action<Scene, LoadSceneMode>(delegate (Scene scene, LoadSceneMode _)
            {
                if (!loaddedAssets)
                {
                    loadAssets();
                    loaddedAssets = true;
                }
                if (!rolesRegistered && scene.name == "MainMenu")
                {
                    Plugin.Roles = RoleManager.Instance.DontDestroy().AllRoles.ToArray().Concat(Plugin.Roles).ToList();
                    foreach (KeyValuePair<Type, RoleTypes> pair in CustomRoleManager.RolesToRegister)
                    {
                        RoleManager.Instance.AllRoles.Add(CustomRoleManager.Register(pair.Key, ModPlugin.GetModPlugin(pair.Key.Assembly), pair.Value));
                    }
                    rolesRegistered = true;
                }
            }));
            Translator errorMessage = new Translator("Failed to sync mods with the host. Please make sure you have the same mods and versions installed as the host.");
            errorMessage.AddTranslation(SupportedLangs.Latam, "Error al sincronizar los mods con el anfitrión. Asegúrate de tener los mismos mods y versiones que el anfitrión.");
            errorMessage.AddTranslation(SupportedLangs.Brazilian, "Houve uma falha ao sincronizar os mods com o host. Certifique-se de ter os mesmos mods e versões instalados que o host.");
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
            DisconnectPopup.ErrorMessages.Add(AmongUsClientPatch.FailedToSyncOptionsError, errorMessage.StringName);
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
                    ModPlugin.Register(plugin, Instance);
                    plugin.ModName = "Vanilla";
                    plugin.ModVersion = ModV;
                    ModPlugin.AllPlugins.Add(plugin);
                }
                return plugin;
            }
        }
        internal static Action loadAssets = new Action(delegate
        {
            RolesSettingMenuPatch.Cog = ResourceHelper.LoadSprite(Plugin, "FungleAPI.Resources.cog", 200f);
            ResourceHelper.EmptySprite = ResourceHelper.LoadSprite(Plugin, "FungleAPI.Resources.empty", 100);
        });
	}
}
