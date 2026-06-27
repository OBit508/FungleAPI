using FungleAPI.Api;
using FungleAPI.Extensions;
using FungleAPI.GModes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace FungleAPI.GameOptions.Lobby
{
    public class VanillaSettingsTab : GameSettingsTab
    {
        public void ClearTab(GameOptionsMenu gameOptionsMenu)
        {
            foreach (CategoryHeaderMasked categoryHeaderMasked in gameOptionsMenu.settingsContainer.GetComponentsInChildren<CategoryHeaderMasked>())
            {
                UnityEngine.Object.Destroy(categoryHeaderMasked.gameObject);
            }
            foreach (OptionBehaviour op in gameOptionsMenu.Children)
            {
                if (op != gameOptionsMenu.MapPicker)
                {
                    UnityEngine.Object.Destroy(op.gameObject);
                }
            }
            gameOptionsMenu.Children.Clear();
            gameOptionsMenu.Children.Add(gameOptionsMenu.MapPicker);
        }
        public override void BuildViewTab(LobbyViewSettingsPane lobbyViewSettingsPane)
        {
            float num = 1.44f;

            BaseGameMode baseGameMode = GameModeManager.GetCurrentGameMode();

            if (baseGameMode.GetType() != typeof(NormalGameMode) && baseGameMode.ModeOptions != null)
            {
                foreach (SettingsGroup group in Plugin.Settings.Groups)
                {
                    CategoryHeaderMasked categoryHeaderMasked = GameObject.Instantiate(lobbyViewSettingsPane.categoryHeaderOrigin);
                    categoryHeaderMasked.SetHeader(group.GroupName, 61);
                    categoryHeaderMasked.transform.SetParent(lobbyViewSettingsPane.settingsContainer);
                    categoryHeaderMasked.transform.localScale = Vector3.one;
                    categoryHeaderMasked.transform.localPosition = new Vector3(-9.77f, num, -2f);
                    lobbyViewSettingsPane.settingsInfo.Add(categoryHeaderMasked.gameObject);
                    num -= 1.05f;
                    for (int i = 0; i < group.Options.Count; i++)
                    {
                        ViewSettingsInfoPanel viewSettingsInfoPanel = GameObject.Instantiate(lobbyViewSettingsPane.infoPanelOrigin);
                        viewSettingsInfoPanel.transform.SetParent(lobbyViewSettingsPane.settingsContainer);
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
                        IModdedOption moddedOption = group.Options.ElementAt(i);
                        if (moddedOption.Data.Type == OptionTypes.Checkbox)
                        {
                            viewSettingsInfoPanel.SetInfoCheckbox(moddedOption.Data.Title, 61, bool.Parse(moddedOption.GetStringValue(AmongUsClient.Instance.AmHost)));
                        }
                        else
                        {
                            viewSettingsInfoPanel.SetInfo(moddedOption.Data.Title, moddedOption.GetStringValue(AmongUsClient.Instance.AmHost), 61);
                        }
                        lobbyViewSettingsPane.settingsInfo.Add(viewSettingsInfoPanel.gameObject);
                    }
                    num -= 0.85f;
                }
            }
            else
            {
                foreach (RulesCategory rulesCategory in GameManager.Instance.GameSettingsList.AllCategories)
                {
                    CategoryHeaderMasked categoryHeaderMasked = GameObject.Instantiate<CategoryHeaderMasked>(lobbyViewSettingsPane.categoryHeaderOrigin);
                    categoryHeaderMasked.SetHeader(rulesCategory.CategoryName, 61);
                    categoryHeaderMasked.transform.SetParent(lobbyViewSettingsPane.settingsContainer);
                    categoryHeaderMasked.transform.localScale = Vector3.one;
                    categoryHeaderMasked.transform.localPosition = new Vector3(-9.77f, num, -2f);
                    lobbyViewSettingsPane.settingsInfo.Add(categoryHeaderMasked.gameObject);
                    num -= 1.05f;
                    List<BaseGameSetting> list = rulesCategory.AllGameSettings.ToSystemList();
                    if (rulesCategory.CategoryName == StringNames.ImpostorsCategory)
                    {
                        if (list.Count > 0) list.RemoveAt(0);
                    }
                    for (int i = 0; i < list.Count; i++)
                    {
                        ViewSettingsInfoPanel viewSettingsInfoPanel = GameObject.Instantiate<ViewSettingsInfoPanel>(lobbyViewSettingsPane.infoPanelOrigin);
                        viewSettingsInfoPanel.transform.SetParent(lobbyViewSettingsPane.settingsContainer);
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
                        lobbyViewSettingsPane.settingsInfo.Add(viewSettingsInfoPanel.gameObject);
                    }
                    num -= 0.85f;
                }
            }
            lobbyViewSettingsPane.scrollBar.CalculateAndSetYBounds(lobbyViewSettingsPane.settingsInfo.Count + 10, 2f, 6f, 0.85f);
        }
        public override void BuildEditTab(GameOptionsMenu gameOptionsMenu)
        {
            float num = 0.713f;

            OptionBehaviour gameModeOption = GameModeManager.CreateGameModeOption(gameOptionsMenu.settingsContainer);
            gameModeOption.LabelBackground.enabled = true;
            gameModeOption.transform.localPosition = new Vector3(0.952f, num, -2f);
            gameModeOption.SetClickMask(gameOptionsMenu.ButtonClickMask);
            gameModeOption.OnValueChanged += new Action<OptionBehaviour>(delegate
            {
                ClearTab(gameOptionsMenu);
                BuildEditTab(gameOptionsMenu);
            });
            gameOptionsMenu.Children.Add(gameModeOption);
            num -= 0.45f;

            BaseGameMode baseGameMode = GameModeManager.GetCurrentGameMode();

            if (baseGameMode.GetType() != typeof(NormalGameMode) && baseGameMode.ModeOptions != null)
            {
                foreach (SettingsGroup group in baseGameMode.ModeOptions.Groups)
                {
                    CategoryHeaderMasked categoryHeaderMasked = UnityEngine.Object.Instantiate(gameOptionsMenu.categoryHeaderOrigin, Vector3.zero, Quaternion.identity, gameOptionsMenu.settingsContainer);
                    categoryHeaderMasked.SetHeader(group.GroupName, 20);
                    categoryHeaderMasked.transform.localScale = Vector3.one * 0.63f;
                    categoryHeaderMasked.transform.localPosition = new Vector3(-0.903f, num, -2f);
                    num -= 0.63f;
                    foreach (IModdedOption option in group.Options)
                    {
                        OptionBehaviour op = option.CreateOption(gameOptionsMenu.settingsContainer);
                        op.LabelBackground.enabled = true;
                        op.transform.localPosition = new Vector3(0.952f, num, -2f);
                        op.SetClickMask(gameOptionsMenu.ButtonClickMask);
                        op.OnValueChanged += new Action<OptionBehaviour>(delegate
                        {
                            SyncManager.RpcSyncOption(option);
                        });
                        gameOptionsMenu.Children.Add(op);
                        num -= 0.45f;
                    }
                }
                gameOptionsMenu.scrollBar.ScrollToTop();
                gameOptionsMenu.scrollBar.SetYBoundsMax(-num - 1.65f);
            }
            else
            {
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
