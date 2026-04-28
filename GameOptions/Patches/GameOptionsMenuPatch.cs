using AmongUs.GameOptions;
using Epic.OnlineServices;
using Epic.OnlineServices.RTC;
using FungleAPI.GameOptions.Lobby;
using FungleAPI.GameOptions.Networking;
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
            AmongUs.GameOptions.GameModes gameModes = GameOptionsManager.Instance.CurrentGameOptions.GameMode;
            if (gameModes == AmongUs.GameOptions.GameModes.HideNSeek && gameModes == AmongUs.GameOptions.GameModes.SeekFools)
            {
                return true;
            }
            Update = true;
            __instance.Initialize();
            return false;
        }
        [HarmonyPatch(nameof(GameOptionsMenu.Initialize))]
        [HarmonyPrefix]
        public static bool InitializePrefix(GameOptionsMenu __instance)
        {
            AmongUs.GameOptions.GameModes gameModes = GameOptionsManager.Instance.CurrentGameOptions.GameMode;
            if (gameModes != AmongUs.GameOptions.GameModes.HideNSeek && gameModes != AmongUs.GameOptions.GameModes.SeekFools)
            {
                try
                {
                    if (__instance.gameObject.active && __instance.Children == null || __instance.Children.Count == 0)
                    {
                        __instance.MapPicker.Initialize(20);
                        BaseGameSetting mapNameSetting = GameManager.Instance.GameSettingsList.MapNameSetting;
                        __instance.MapPicker.SetUpFromData(mapNameSetting, 20);
                        __instance.Children = new Il2CppSystem.Collections.Generic.List<OptionBehaviour>();
                        __instance.Children.Add(__instance.MapPicker);
                        BuildTab(__instance);
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
                        BuildTab(__instance);
                        Update = false;
                    }
                    __instance.MapPicker.gameObject.SetActive(GameSettingMenuPatch.CurrentTab is GameSettingsTab gameSettingsTab && gameSettingsTab.Plugin == FungleAPIPlugin.Plugin);
                }
                catch { }
                return false;
            }
            return true;
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

            if (GameSettingMenuPatch.pluginChanger.CurrentPlugin == FungleAPIPlugin.Plugin) return true;

            __instance.RefreshChildren();
            __instance.RolesMenu.RefreshChildren();

            return false;
        }
        public static void BuildTab(GameOptionsMenu gameOptionsMenu)
        {
            if (!(GameSettingMenuPatch.CurrentTab is GameSettingsTab gameSettingsTab && gameSettingsTab.Plugin == FungleAPIPlugin.Plugin))
            {
                GameSettingMenuPatch.CurrentTab.BuildEditTab(gameOptionsMenu);
            }
            else
            {
                float num = 0.713f;
                foreach (RulesCategory rulesCategory in GameManager.Instance.GameSettingsList.AllCategories)
                {
                    CategoryHeaderMasked categoryHeaderMasked = GameObject.Instantiate<CategoryHeaderMasked>(gameOptionsMenu.categoryHeaderOrigin, Vector3.zero, Quaternion.identity, gameOptionsMenu.settingsContainer);
                    categoryHeaderMasked.SetHeader(rulesCategory.CategoryName, 20);
                    categoryHeaderMasked.transform.localScale = Vector3.one * 0.63f;
                    categoryHeaderMasked.transform.localPosition = new Vector3(-0.903f, num, -2f);
                    num -= 0.63f;
                    foreach (BaseGameSetting baseGameSetting in rulesCategory.AllGameSettings)
                    {
                        if (rulesCategory.CategoryName == StringNames.ImpostorsCategory && baseGameSetting != rulesCategory.AllGameSettings[0] || rulesCategory.CategoryName != StringNames.ImpostorsCategory)
                        {
                            OptionBehaviour optionBehaviour = null;
                            switch (baseGameSetting.Type)
                            {
                                case OptionTypes.Checkbox:
                                    {
                                        optionBehaviour = GameObject.Instantiate<ToggleOption>(gameOptionsMenu.checkboxOrigin, Vector3.zero, Quaternion.identity, gameOptionsMenu.settingsContainer);
                                        optionBehaviour.transform.localPosition = new Vector3(0.952f, num, -2f);
                                        optionBehaviour.SetClickMask(gameOptionsMenu.ButtonClickMask);
                                        optionBehaviour.SetUpFromData(baseGameSetting, 20);
                                        gameOptionsMenu.Children.Add(optionBehaviour);
                                        break;
                                    }
                                case OptionTypes.String:
                                    {
                                        optionBehaviour = GameObject.Instantiate<StringOption>(gameOptionsMenu.stringOptionOrigin, Vector3.zero, Quaternion.identity, gameOptionsMenu.settingsContainer);
                                        optionBehaviour.transform.localPosition = new Vector3(0.952f, num, -2f);
                                        optionBehaviour.SetClickMask(gameOptionsMenu.ButtonClickMask);
                                        optionBehaviour.SetUpFromData(baseGameSetting, 20);
                                        gameOptionsMenu.Children.Add(optionBehaviour);
                                        break;
                                    }
                                case OptionTypes.Float:
                                case OptionTypes.Int:
                                    {
                                        optionBehaviour = GameObject.Instantiate<NumberOption>(gameOptionsMenu.numberOptionOrigin, Vector3.zero, Quaternion.identity, gameOptionsMenu.settingsContainer);
                                        optionBehaviour.transform.localPosition = new Vector3(0.952f, num, -2f);
                                        optionBehaviour.SetClickMask(gameOptionsMenu.ButtonClickMask);
                                        optionBehaviour.SetUpFromData(baseGameSetting, 20);
                                        gameOptionsMenu.Children.Add(optionBehaviour);
                                        break;
                                    }
                                case OptionTypes.Player:
                                    {
                                        optionBehaviour = GameObject.Instantiate<PlayerOption>(gameOptionsMenu.playerOptionOrigin, Vector3.zero, Quaternion.identity, gameOptionsMenu.settingsContainer);
                                        optionBehaviour.transform.localPosition = new Vector3(0.952f, num, -2f);
                                        optionBehaviour.SetClickMask(gameOptionsMenu.ButtonClickMask);
                                        optionBehaviour.SetUpFromData(baseGameSetting, 20);
                                        gameOptionsMenu.Children.Add(optionBehaviour);
                                        break;
                                    }
                            }
                            if (optionBehaviour != null)
                            {
                                optionBehaviour.OnValueChanged = new Action<OptionBehaviour>(gameOptionsMenu.ValueChanged);
                            }
                            num -= 0.45f;
                        }
                    }
                }
                gameOptionsMenu.scrollBar.ScrollToTop();
                gameOptionsMenu.scrollBar.SetYBoundsMax(-num - 1.65f);
            }
        }
    }
}
