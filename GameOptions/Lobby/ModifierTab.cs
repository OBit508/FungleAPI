using AmongUs.GameOptions;
using Epic.OnlineServices.IntegratedPlatform;
using FungleAPI.Api;
using FungleAPI.Modifiers;
using FungleAPI.Teams;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace FungleAPI.GameOptions.Lobby
{
    public class ModifierTab : FungleTab
    {
        public override string ViewTabButtonText => FungleTranslation.ModifiersText.GetString();
        public override string EditTabButtonText => FungleTranslation.ModifierConfigButtonText.GetString();
        public override string TabDescriptionText => FungleTranslation.ModifierConfigDescText.GetString();
        public override void BuildViewTab(LobbyViewSettingsPane lobbyViewSettingsPane)
        {
            lobbyViewSettingsPane.scrollBar.ScrollToTop();
            float num = 1.44f;
            foreach (BaseModifier baseModifier in Plugin.Modifiers)
            {
                if (!baseModifier.HideInLobby)
                {
                    CategoryHeaderMasked categoryHeaderMasked = GameObject.Instantiate(lobbyViewSettingsPane.categoryHeaderOrigin);
                    categoryHeaderMasked.SetHeader(baseModifier.ModifierName, 61);
                    categoryHeaderMasked.transform.SetParent(lobbyViewSettingsPane.settingsContainer);
                    categoryHeaderMasked.transform.localScale = Vector3.one;
                    categoryHeaderMasked.transform.localPosition = new Vector3(-9.77f, num, -2f);
                    lobbyViewSettingsPane.settingsInfo.Add(categoryHeaderMasked.gameObject);
                    num -= 1.05f;
                    for (int i = 0; i < baseModifier.ModifierOptions.Options.Count + 2; i++)
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
                        if (i > 1)
                        {
                            IModdedOption moddedOption = baseModifier.ModifierOptions.Options.ElementAt(i - 2);
                            if (moddedOption.Data.Type == OptionTypes.Checkbox)
                            {
                                viewSettingsInfoPanel.SetInfoCheckbox(moddedOption.Data.Title, 61, bool.Parse(moddedOption.GetStringValue(AmongUsClient.Instance.AmHost)));
                            }
                            else
                            {
                                viewSettingsInfoPanel.SetInfo(moddedOption.Data.Title, moddedOption.GetStringValue(AmongUsClient.Instance.AmHost), 61);
                            }
                        }
                        else if (i == 0)
                        {
                            viewSettingsInfoPanel.SetInfo(baseModifier.CountData.Title, baseModifier.GetCount().ToString(), 61);
                        }
                        else if (i == 1)
                        {
                            viewSettingsInfoPanel.SetInfo(baseModifier.ChanceData.Title, baseModifier.GetChance().ToString(), 61);
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
            float num = 2;
            foreach (BaseModifier baseModifier in Plugin.Modifiers)
            {
                if (!baseModifier.HideInLobby)
                {
                    CategoryHeaderMasked categoryHeaderMasked = GameObject.Instantiate(gameOptionsMenu.categoryHeaderOrigin, Vector3.zero, Quaternion.identity, gameOptionsMenu.settingsContainer);
                    categoryHeaderMasked.SetHeader(baseModifier.ModifierName, 20);
                    categoryHeaderMasked.transform.localScale = Vector3.one * 0.63f;
                    categoryHeaderMasked.transform.localPosition = new Vector3(-0.903f, num, -2f);
                    num -= 0.63f;
                    OptionBehaviour count = CreateCountOption(gameOptionsMenu.settingsContainer, baseModifier);
                    count.LabelBackground.enabled = true;
                    count.transform.localPosition = new Vector3(0.952f, num, -2f);
                    count.SetClickMask(gameOptionsMenu.ButtonClickMask);
                    count.OnValueChanged += new Action<OptionBehaviour>(delegate
                    {
                        SyncManager.RpcSyncModifier(baseModifier);
                    });
                    gameOptionsMenu.Children.Add(count);
                    num -= 0.45f;
                    OptionBehaviour priority = CreatePriorityOption(gameOptionsMenu.settingsContainer, baseModifier);
                    priority.LabelBackground.enabled = true;
                    priority.transform.localPosition = new Vector3(0.952f, num, -2f);
                    priority.SetClickMask(gameOptionsMenu.ButtonClickMask);
                    priority.OnValueChanged += new Action<OptionBehaviour>(delegate
                    {
                        SyncManager.RpcSyncModifier(baseModifier);
                    });
                    gameOptionsMenu.Children.Add(priority);
                    num -= 0.45f;
                    foreach (IModdedOption option in baseModifier.ModifierOptions.Options)
                    {
                        OptionBehaviour op = option.CreateOption(gameOptionsMenu.settingsContainer);
                        op.LabelBackground.enabled = true;
                        op.transform.localPosition = new Vector3(0.952f, num, -2f);
                        op.SetClickMask(gameOptionsMenu.ButtonClickMask);
                        op.OnValueChanged += new Action<OptionBehaviour>(delegate
                        {
                            SyncManager.RpcSyncModifierOption(baseModifier, option);
                        });
                        gameOptionsMenu.Children.Add(op);
                        num -= 0.45f;
                    }
                }
            }
            gameOptionsMenu.scrollBar.ScrollToTop();
            gameOptionsMenu.scrollBar.SetYBoundsMax(-num - 1.65f);
        }
        public virtual OptionBehaviour CreateCountOption(Transform transform, BaseModifier baseModifier)
        {
            NumberOption option = null;
            option = OptionManager.CreateNumberOption(transform, baseModifier.CountData, delegate
            {
                baseModifier.ModifierOptions.SetLocal((byte)option.Value, baseModifier.ModifierOptions.LocalModifierChance.Value);
            });
            baseModifier.CountData.Value = baseModifier.ModifierOptions.LocalModifierCount.Value;
            option.Value = baseModifier.ModifierOptions.LocalModifierCount.Value;
            return option;
        }
        public virtual OptionBehaviour CreatePriorityOption(Transform transform, BaseModifier baseModifier)
        {
            NumberOption option = null;
            option = OptionManager.CreateNumberOption(transform, baseModifier.ChanceData, delegate
            {
                baseModifier.ModifierOptions.SetLocal(baseModifier.ModifierOptions.LocalModifierCount.Value, (byte)option.Value);
            });
            baseModifier.ChanceData.Value = baseModifier.ModifierOptions.LocalModifierChance.Value;
            option.Value = baseModifier.ModifierOptions.LocalModifierChance.Value;
            return option;
        }
    }
}
