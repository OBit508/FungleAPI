using AmongUs.GameOptions;
using Epic.OnlineServices;
using Epic.OnlineServices.RTC;
using FungleAPI.Api;
using FungleAPI.GameOptions.Lobby;
using FungleAPI.GameOptions.Networking;
using FungleAPI.GModes;
using FungleAPI.Networking;
using FungleAPI.PluginLoading;
using FungleAPI.Teams;
using FungleAPI.Utilities;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace FungleAPI.GameOptions.Patches
{
    [HarmonyPatch(typeof(GameOptionsMenu))]
    internal static class GameOptionsMenuPatch
    {
        public static bool Update;
        [HarmonyPatch(nameof(GameOptionsMenu.RefreshChildren))]
        [HarmonyPrefix]
        public static bool RefreshChildrenPrefix(GameOptionsMenu __instance)
        {
            if (GameManager.Instance.IsHideAndSeek()) return true;

            Update = true;
            __instance.Initialize();
            return false;
        }
        [HarmonyPatch(nameof(GameOptionsMenu.Initialize))]
        [HarmonyPrefix]
        public static bool InitializePrefix(GameOptionsMenu __instance)
        {
            try
            {
                if (GameManager.Instance.IsHideAndSeek()) return true;

                if (__instance.gameObject.active && __instance.Children == null || __instance.Children.Count == 0)
                {
                    __instance.MapPicker.Initialize(20);
                    BaseGameSetting mapNameSetting = GameManager.Instance.GameSettingsList.MapNameSetting;
                    __instance.MapPicker.SetUpFromData(mapNameSetting, 20);
                    __instance.Children = new Il2CppSystem.Collections.Generic.List<OptionBehaviour>();
                    __instance.Children.Add(__instance.MapPicker);
                    GameSettingMenuPatch.CurrentTab.BuildEditTab(__instance);
                    __instance.cachedData = GameOptionsManager.Instance.CurrentGameOptions;
                    for (int i = 0; i < __instance.Children.Count; i++)
                    {
                        OptionBehaviour optionBehaviour = __instance.Children[i];
                        if (AmongUsClient.Instance && !AmongUsClient.Instance.AmHost)
                        {
                            optionBehaviour.SetAsPlayer();
                        }
                    }
                    __instance.InitializeControllerNavigation();
                }
                else if (Update)
                {
                    foreach (CategoryHeaderMasked categoryHeaderMasked in __instance.settingsContainer.GetComponentsInChildren<CategoryHeaderMasked>())
                    {
                        UnityEngine.Object.Destroy(categoryHeaderMasked.gameObject);
                    }
                    foreach (OptionBehaviour op in __instance.Children)
                    {
                        if (op != __instance.MapPicker)
                        {
                            UnityEngine.Object.Destroy(op.gameObject);
                        }
                    }
                    __instance.Children.Clear();
                    __instance.Children.Add(__instance.MapPicker);
                    try
                    {
                        GameSettingMenuPatch.CurrentTab.BuildEditTab(__instance);
                    }
                    catch (Exception ex) { Debug.LogError(ex.Message); }
                    Update = false;
                }
                __instance.MapPicker.gameObject.SetActive(GameSettingMenuPatch.CurrentTab is GameSettingsTab gameSettingsTab && gameSettingsTab.Plugin == FungleApiPlugin.Plugin);
                return false;
            }
            catch { return true; }
        }
        [HarmonyPatch(nameof(GameOptionsMenu.ClickPresetButton))]
        [HarmonyPrefix]
        public static bool ClickPresetPrefix(GameOptionsMenu __instance, RulesPresets preset)
        {
            if (GameManager.Instance.IsHideAndSeek()) return true;

            IFungleBasePlugin fungleBasePlugin = GameSettingMenuPatch.pluginChanger.CurrentPlugin.BasePlugin as IFungleBasePlugin;
            if (fungleBasePlugin != null)
            {
                fungleBasePlugin.SetPreset(preset, GameSettingMenuPatch.pluginChanger.CurrentPlugin);
                SyncManager.RpcUpdatePreset(preset, GameSettingMenuPatch.pluginChanger.CurrentPlugin);
            }

            if (GameSettingMenuPatch.pluginChanger.CurrentPlugin == FungleApiPlugin.Plugin) return true;

            __instance.RefreshChildren();
            __instance.RolesMenu.RefreshChildren();

            return false;
        }
    }
}
