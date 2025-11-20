using Epic.OnlineServices;
using Epic.OnlineServices.RTC;
using FungleAPI.Configuration.Attributes;
using FungleAPI.Networking;
using FungleAPI.Networking.RPCs;
using FungleAPI.PluginLoading;
using FungleAPI.Role.Teams;
using FungleAPI.Utilities;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace FungleAPI.Configuration.Patches
{
    [HarmonyPatch(typeof(GameOptionsMenu))]
    internal static class GameOptionsMenuPatch
    {
        public static ModPlugin LastPlugin;
        public static bool LastCheck;
        public static bool Update;
        [HarmonyPatch("RefreshChildren")]
        [HarmonyPrefix]
        public static bool RefreshChildrenPrefix(GameOptionsMenu __instance)
        {
            if (GameSettingMenuPatch.pluginChanger.CurrentPlugin == FungleAPIPlugin.Plugin)
            {
                return true;
            }
            Update = true;
            __instance.Initialize();
            return false;
        }
        [HarmonyPatch("Initialize")]
        [HarmonyPrefix]
        public static bool InitializePrefix(GameOptionsMenu __instance)
        {
            if (GameOptionsManager.Instance.CurrentGameOptions.GameMode != AmongUs.GameOptions.GameModes.HideNSeek && GameOptionsManager.Instance.CurrentGameOptions.GameMode != AmongUs.GameOptions.GameModes.SeekFools)
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
                        Set(__instance);
                        LastPlugin = GameSettingMenuPatch.pluginChanger.CurrentPlugin;
                        LastCheck = GameSettingMenuPatch.TeamConfigTab;
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
                    else if (LastPlugin != GameSettingMenuPatch.pluginChanger.CurrentPlugin || LastCheck != GameSettingMenuPatch.TeamConfigTab || Update)
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
                        Set(__instance);
                        LastPlugin = GameSettingMenuPatch.pluginChanger.CurrentPlugin;
                        LastCheck = GameSettingMenuPatch.TeamConfigTab;
                        Update = false;
                    }
                    __instance.MapPicker.gameObject.SetActive(GameSettingMenuPatch.pluginChanger.CurrentPlugin == FungleAPIPlugin.Plugin && !GameSettingMenuPatch.TeamConfigTab);
                }
                catch { }
                return false;
            }
            return true;
        }
        public static void Set(GameOptionsMenu menu)
        {
            try
            {
                if (GameSettingMenuPatch.TeamConfigTab)
                {
                    CreateTeamConfigTab(menu);
                }
                else
                {
                    if (GameSettingMenuPatch.pluginChanger.CurrentPlugin == FungleAPIPlugin.Plugin)
                    {
                        float num = 0.713f;
                        foreach (RulesCategory rulesCategory in GameManager.Instance.GameSettingsList.AllCategories)
                        {
                            CategoryHeaderMasked categoryHeaderMasked = GameObject.Instantiate<CategoryHeaderMasked>(menu.categoryHeaderOrigin, Vector3.zero, Quaternion.identity, menu.settingsContainer);
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
                                                optionBehaviour = GameObject.Instantiate<ToggleOption>(menu.checkboxOrigin, Vector3.zero, Quaternion.identity, menu.settingsContainer);
                                                optionBehaviour.transform.localPosition = new Vector3(0.952f, num, -2f);
                                                optionBehaviour.SetClickMask(menu.ButtonClickMask);
                                                optionBehaviour.SetUpFromData(baseGameSetting, 20);
                                                menu.Children.Add(optionBehaviour);
                                                break;
                                            }
                                        case OptionTypes.String:
                                            {
                                                optionBehaviour = GameObject.Instantiate<StringOption>(menu.stringOptionOrigin, Vector3.zero, Quaternion.identity, menu.settingsContainer);
                                                optionBehaviour.transform.localPosition = new Vector3(0.952f, num, -2f);
                                                optionBehaviour.SetClickMask(menu.ButtonClickMask);
                                                optionBehaviour.SetUpFromData(baseGameSetting, 20);
                                                menu.Children.Add(optionBehaviour);
                                                break;
                                            }
                                        case OptionTypes.Float:
                                        case OptionTypes.Int:
                                            {
                                                optionBehaviour = GameObject.Instantiate<NumberOption>(menu.numberOptionOrigin, Vector3.zero, Quaternion.identity, menu.settingsContainer);
                                                optionBehaviour.transform.localPosition = new Vector3(0.952f, num, -2f);
                                                optionBehaviour.SetClickMask(menu.ButtonClickMask);
                                                optionBehaviour.SetUpFromData(baseGameSetting, 20);
                                                menu.Children.Add(optionBehaviour);
                                                break;
                                            }
                                        case OptionTypes.Player:
                                            {
                                                optionBehaviour = GameObject.Instantiate<PlayerOption>(menu.playerOptionOrigin, Vector3.zero, Quaternion.identity, menu.settingsContainer);
                                                optionBehaviour.transform.localPosition = new Vector3(0.952f, num, -2f);
                                                optionBehaviour.SetClickMask(menu.ButtonClickMask);
                                                optionBehaviour.SetUpFromData(baseGameSetting, 20);
                                                menu.Children.Add(optionBehaviour);
                                                break;
                                            }
                                    }
                                    if (optionBehaviour != null)
                                    {
                                        optionBehaviour.OnValueChanged = new Action<OptionBehaviour>(menu.ValueChanged);
                                    }
                                    num -= 0.45f;
                                }
                            }
                        }
                        menu.scrollBar.ScrollToTop();
                        menu.scrollBar.SetYBoundsMax(-num - 1.65f);
                    }
                    else
                    {
                        CreateSettings(menu);
                    }
                }
            }
            catch (Exception e)
            {
                FungleAPIPlugin.Instance.Log.LogError(e);
            }
        }
        public static void CreateSettings(GameOptionsMenu menu)
        {
            float num = 2;
            foreach (SettingsGroup group in GameSettingMenuPatch.pluginChanger.CurrentPlugin.Settings.Groups)
            {
                CategoryHeaderMasked categoryHeaderMasked = UnityEngine.Object.Instantiate(menu.categoryHeaderOrigin, Vector3.zero, Quaternion.identity, menu.settingsContainer);
                categoryHeaderMasked.SetHeader(group.GroupName, 20);
                categoryHeaderMasked.transform.localScale = Vector3.one * 0.63f;
                categoryHeaderMasked.transform.localPosition = new Vector3(-0.903f, num, -2f);
                num -= 0.63f;
                foreach (ModdedOption option in group.Options)
                {
                    OptionBehaviour op = option.CreateOption(menu.settingsContainer);
                    op.LabelBackground.enabled = true;
                    op.transform.localPosition = new Vector3(0.952f, num, -2f);
                    op.SetClickMask(menu.ButtonClickMask);
                    op.OnValueChanged += new Action<OptionBehaviour>(delegate
                    {
                        CustomRpcManager.Instance<RpcSyncSettings>().Send((SyncTextType.Option, option, null, null), PlayerControl.LocalPlayer.NetId);
                    });
                    menu.Children.Add(op);
                    num -= 0.45f;
                }
            }
            menu.scrollBar.ScrollToTop();
            menu.scrollBar.SetYBoundsMax(-num - 1.65f);
        }
        public static void CreateTeamConfigTab(GameOptionsMenu menu)
        {
            float num = 2;
            foreach (ModdedTeam team in GameSettingMenuPatch.pluginChanger.CurrentPlugin.Teams)
            {
                if (team != ModdedTeam.Crewmates)
                {
                    CategoryHeaderMasked categoryHeaderMasked = UnityEngine.Object.Instantiate(menu.categoryHeaderOrigin, Vector3.zero, Quaternion.identity, menu.settingsContainer);
                    categoryHeaderMasked.SetHeader(team.TeamName, 20);
                    categoryHeaderMasked.transform.localScale = Vector3.one * 0.63f;
                    categoryHeaderMasked.transform.localPosition = new Vector3(-0.903f, num, -2f);
                    num -= 0.63f;
                    OptionBehaviour count = team.CreateCountOption(menu.settingsContainer);
                    count.LabelBackground.enabled = true;
                    count.transform.localPosition = new Vector3(0.952f, num, -2f);
                    count.SetClickMask(menu.ButtonClickMask);
                    count.OnValueChanged += new Action<OptionBehaviour>(delegate
                    {
                        CustomRpcManager.Instance<RpcSyncSettings>().Send((SyncTextType.TeamCount, null, null, team), PlayerControl.LocalPlayer.NetId);
                    });
                    menu.Children.Add(count);
                    num -= 0.45f;
                    OptionBehaviour priority = team.CreatePriorityOption(menu.settingsContainer);
                    priority.LabelBackground.enabled = true;
                    priority.transform.localPosition = new Vector3(0.952f, num, -2f);
                    priority.SetClickMask(menu.ButtonClickMask);
                    priority.OnValueChanged += new Action<OptionBehaviour>(delegate
                    {
                        CustomRpcManager.Instance<RpcSyncSettings>().Send((SyncTextType.TeamPriority, null, null, team), PlayerControl.LocalPlayer.NetId);
                    });
                    menu.Children.Add(priority);
                    num -= 0.45f;
                    foreach (ModdedOption option in team.ExtraConfigs)
                    {
                        OptionBehaviour op = option.CreateOption(menu.settingsContainer);
                        op.LabelBackground.enabled = true;
                        op.transform.localPosition = new Vector3(0.952f, num, -2f);
                        op.SetClickMask(menu.ButtonClickMask);
                        op.OnValueChanged += new Action<OptionBehaviour>(delegate
                        {
                            CustomRpcManager.Instance<RpcSyncSettings>().Send((SyncTextType.TeamOption, option, null, team), PlayerControl.LocalPlayer.NetId);
                        });
                        menu.Children.Add(op);
                        num -= 0.45f;
                    }
                }
            }
            menu.scrollBar.ScrollToTop();
            menu.scrollBar.SetYBoundsMax(-num - 1.65f);
        }
    }
}
