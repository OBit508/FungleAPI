using AmongUs.GameOptions;
using FungleAPI.Components;
using FungleAPI.GameMode;
using FungleAPI.GameOptions;
using FungleAPI.GameOptions.Lobby;
using FungleAPI.PluginLoading;
using FungleAPI.Role;
using FungleAPI.Role.Utilities;
using FungleAPI.Teams;
using FungleAPI.Translation;
using FungleAPI.Utilities;
using FungleAPI.Utilities.Assets;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using xCloud;
using static Il2CppMono.Security.X509.X520;
using static UnityEngine.GraphicsBuffer;

namespace FungleAPI.GameOptions.Patches
{
    [HarmonyPatch(typeof(LobbyViewSettingsPane))]
    internal static class LobbyViewSettingsPanePatch
    {
        public static List<LobbyTab> Tabs = new List<LobbyTab>();
        public static LobbyTab Tab;

        public static PluginChanger pluginChanger;

        [HarmonyPatch(nameof(LobbyViewSettingsPane.Awake))]
        [HarmonyPostfix]
        public static void AwakePostfix(LobbyViewSettingsPane __instance)
        {
            if (GameOptionsManager.Instance.CurrentGameOptions.GameMode == GameModes.HideNSeek || GameOptionsManager.Instance.CurrentGameOptions.GameMode == GameModes.SeekFools) return;

            __instance.gameModeText.gameObject.SetActive(false);
            pluginChanger = FungleAssets.PluginChangerPrefab.Instantiate(__instance.rolesTabButton.transform.parent).GetComponent<PluginChanger>();
            pluginChanger.transform.localPosition = __instance.gameModeText.transform.localPosition;
            pluginChanger.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);

            UiElement buttonPrefab = GameObject.Instantiate(__instance.ControllerSelectable[3], __instance.transform);
            buttonPrefab.gameObject.SetActive(false);

            pluginChanger.OnChange = new Action<ModPlugin>(delegate (ModPlugin plugin)
            {
                foreach (UiElement uiElement in __instance.ControllerSelectable)
                {
                    uiElement.Destroy();
                }
                __instance.ControllerSelectable.Clear();

                Tabs = plugin.LobbyTabs;
                Tab = Tabs[0];

                foreach (LobbyTab lobbyTab in Tabs)
                {
                    lobbyTab.ViewSettingsButton = CreateButton(__instance, lobbyTab.ViewTabButtonText, delegate
                    {
                        Tab = lobbyTab;
                        __instance.RefreshTab();
                    });
                }

                float num = 0;
                int index = 0;
                for (int i = 0; i < __instance.ControllerSelectable.Count; i++)
                {
                    UiElement uiElement = __instance.ControllerSelectable[i];
                    if (uiElement != null && uiElement.isActiveAndEnabled)
                    {
                        uiElement.transform.localPosition = new Vector3(-4.871f + (index * 3.471f), 1.404f, 0);
                        num += 1f;
                        index++;
                    }
                }

                __instance.scrollBar.ScrollToTop();
            });

            pluginChanger.OnChange(FungleAPIPlugin.Plugin);
        }
        public static PassiveButton CreateButton(LobbyViewSettingsPane lobbyViewSettingsPane, string name, Action onClick)
        {
            PassiveButton passiveButton = GameObject.Instantiate<PassiveButton>(lobbyViewSettingsPane.rolesTabButton, lobbyViewSettingsPane.transform);
            passiveButton.buttonText.GetComponent<TextTranslatorTMP>().enabled = false;
            passiveButton.buttonText.text = name;
            passiveButton.SetNewAction(onClick);
            lobbyViewSettingsPane.ControllerSelectable.Add(passiveButton);
            return passiveButton;
        }
        [HarmonyPatch(nameof(LobbyViewSettingsPane.SetTab))]
        [HarmonyPrefix]
        public static bool DrawTabPrefix(LobbyViewSettingsPane __instance)
        {
            foreach (LobbyTab lobbyTab in Tabs)
            {
                lobbyTab.ViewSettingsButton.SelectButton(false);
            }
            Tab.ViewSettingsButton.SelectButton(true);
            if (!(Tab is GameSettingsTab gameSettingsTab && gameSettingsTab.Plugin == FungleAPIPlugin.Plugin))
            {
                Tab.BuildViewTab(__instance);
            }
            else
            {
                float num = 1.44f;
                foreach (RulesCategory rulesCategory in GameManager.Instance.GameSettingsList.AllCategories)
                {
                    CategoryHeaderMasked categoryHeaderMasked = GameObject.Instantiate<CategoryHeaderMasked>(__instance.categoryHeaderOrigin);
                    categoryHeaderMasked.SetHeader(rulesCategory.CategoryName, 61);
                    categoryHeaderMasked.transform.SetParent(__instance.settingsContainer);
                    categoryHeaderMasked.transform.localScale = Vector3.one;
                    categoryHeaderMasked.transform.localPosition = new Vector3(-9.77f, num, -2f);
                    __instance.settingsInfo.Add(categoryHeaderMasked.gameObject);
                    num -= 1.05f;
                    List<BaseGameSetting> list = rulesCategory.AllGameSettings.ToSystemList();
                    if (rulesCategory.CategoryName == StringNames.ImpostorsCategory)
                    {
                        if (list.Count > 0) list.RemoveAt(0);
                    }
                    for (int i = 0; i < list.Count; i++)
                    {
                        ViewSettingsInfoPanel viewSettingsInfoPanel = GameObject.Instantiate<ViewSettingsInfoPanel>(__instance.infoPanelOrigin);
                        viewSettingsInfoPanel.transform.SetParent(__instance.settingsContainer);
                        viewSettingsInfoPanel.transform.localScale = Vector3.one;
                        float num2;
                        if (i % 2 == 0)
                        {
                            num2 = -8.95f;
                            if (i > 0)
                            {
                                num -= 0.85f;
                            }
                        }
                        else
                        {
                            num2 = -3f;
                        }
                        viewSettingsInfoPanel.transform.localPosition = new Vector3(num2, num, -2f);
                        float value = GameOptionsManager.Instance.CurrentGameOptions.GetValue(list[i]);
                        if (list[i].Type == OptionTypes.Checkbox)
                        {
                            viewSettingsInfoPanel.SetInfoCheckbox(list[i].Title, 61, value > 0f);
                        }
                        else
                        {
                            viewSettingsInfoPanel.SetInfo(list[i].Title, list[i].GetValueString(value), 61);
                        }
                        __instance.settingsInfo.Add(viewSettingsInfoPanel.gameObject);
                    }
                    num -= 0.85f;
                }
                __instance.scrollBar.CalculateAndSetYBounds((float)(__instance.settingsInfo.Count + 10), 2f, 6f, 0.85f);
            }
            return false;
        }
    }
}