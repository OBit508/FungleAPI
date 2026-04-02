using FungleAPI.Configuration.Attributes;
using FungleAPI.Teams;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace FungleAPI.Configuration.Patches
{
    internal static class TabBuilder
    {
        public static void BuildTab(GameOptionsMenu gameOptionsMenu, TabType tabType)
        {
            switch (tabType)
            {
                case TabType.TeamTab: BuildTeamTab(gameOptionsMenu); break;
                case TabType.SetttingsTab: BuildSettingsTab(gameOptionsMenu); break;
                case TabType.VanillaSettingsTab: BuildVanillaSettingsTab(gameOptionsMenu); break;
            }
        }
        public static void BuildSettingsTab(GameOptionsMenu gameOptionsMenu)
        {
            float num = 2;
            foreach (SettingsGroup group in GameSettingMenuPatch.pluginChanger.CurrentPlugin.Settings.Groups)
            {
                CategoryHeaderMasked categoryHeaderMasked = UnityEngine.Object.Instantiate(gameOptionsMenu.categoryHeaderOrigin, Vector3.zero, Quaternion.identity, gameOptionsMenu.settingsContainer);
                categoryHeaderMasked.SetHeader(group.GroupName.StringName, 20);
                categoryHeaderMasked.transform.localScale = Vector3.one * 0.63f;
                categoryHeaderMasked.transform.localPosition = new Vector3(-0.903f, num, -2f);
                num -= 0.63f;
                foreach (ModdedOption option in group.Options)
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
        public static void BuildTeamTab(GameOptionsMenu gameOptionsMenu)
        {
            float num = 2;
            foreach (ModdedTeam team in GameSettingMenuPatch.pluginChanger.CurrentPlugin.Teams)
            {
                if (team != ModdedTeamManager.Crewmates)
                {
                    CategoryHeaderMasked categoryHeaderMasked = UnityEngine.Object.Instantiate(gameOptionsMenu.categoryHeaderOrigin, Vector3.zero, Quaternion.identity, gameOptionsMenu.settingsContainer);
                    categoryHeaderMasked.SetHeader(team.TeamName, 20);
                    categoryHeaderMasked.transform.localScale = Vector3.one * 0.63f;
                    categoryHeaderMasked.transform.localPosition = new Vector3(-0.903f, num, -2f);
                    num -= 0.63f;
                    OptionBehaviour count = team.CreateCountOption(gameOptionsMenu.settingsContainer);
                    count.LabelBackground.enabled = true;
                    count.transform.localPosition = new Vector3(0.952f, num, -2f);
                    count.SetClickMask(gameOptionsMenu.ButtonClickMask);
                    count.OnValueChanged += new Action<OptionBehaviour>(delegate
                    {
                        SyncManager.RpcSyncTeam(team);
                    });
                    gameOptionsMenu.Children.Add(count);
                    num -= 0.45f;
                    OptionBehaviour priority = team.CreatePriorityOption(gameOptionsMenu.settingsContainer);
                    priority.LabelBackground.enabled = true;
                    priority.transform.localPosition = new Vector3(0.952f, num, -2f);
                    priority.SetClickMask(gameOptionsMenu.ButtonClickMask);
                    priority.OnValueChanged += new Action<OptionBehaviour>(delegate
                    {
                        SyncManager.RpcSyncTeam(team);
                    });
                    gameOptionsMenu.Children.Add(priority);
                    num -= 0.45f;
                    foreach (ModdedOption option in team.TeamOptions.ExtraOptions)
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
            }
            gameOptionsMenu.scrollBar.ScrollToTop();
            gameOptionsMenu.scrollBar.SetYBoundsMax(-num - 1.65f);
        }
        public static void BuildVanillaSettingsTab(GameOptionsMenu gameOptionsMenu)
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
