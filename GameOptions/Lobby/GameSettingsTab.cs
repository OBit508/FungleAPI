using FungleAPI.Components;
using FungleAPI.GameOptions.Patches;
using FungleAPI.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace FungleAPI.GameOptions.Lobby
{
    public class GameSettingsTab : LobbyTab
    {
        public override string ViewTabButtonText => StringNames.OverviewCategory.GetString();
        public override string EditTabButtonText => StringNames.GameSettingsLabel.GetString();
        public override string TabDescriptionText => StringNames.GameSettingsDescription.GetString();
        public override void BuildViewTab(LobbyViewSettingsPane lobbyViewSettingsPane)
        {
            float num = 1.44f;
            foreach (SettingsGroup group in Plugin.Settings.Groups)
            {
                CategoryHeaderMasked categoryHeaderMasked = GameObject.Instantiate(lobbyViewSettingsPane.categoryHeaderOrigin);
                categoryHeaderMasked.SetHeader(group.GroupName, 61);
                categoryHeaderMasked.transform.SetParent(lobbyViewSettingsPane.settingsContainer);
                categoryHeaderMasked.transform.localScale = Vector3.one;
                categoryHeaderMasked.transform.localPosition = new Vector3(-9.77f, num, -2f);
                lobbyViewSettingsPane.settingsInfo.Add(categoryHeaderMasked.gameObject);
                num -= 1.05f;
                for (int i = 0; i < group.OptionCollection.Options.Count; i++)
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
                    IModdedOption moddedOption = group.OptionCollection.Options.Values.ElementAt(i);
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
            lobbyViewSettingsPane.scrollBar.CalculateAndSetYBounds(lobbyViewSettingsPane.settingsInfo.Count + 10, 2f, 6f, 0.85f);
        }
        public override void BuildEditTab(GameOptionsMenu gameOptionsMenu)
        {
            float num = 2;
            foreach (SettingsGroup group in Plugin.Settings.Groups)
            {
                CategoryHeaderMasked categoryHeaderMasked = UnityEngine.Object.Instantiate(gameOptionsMenu.categoryHeaderOrigin, Vector3.zero, Quaternion.identity, gameOptionsMenu.settingsContainer);
                categoryHeaderMasked.SetHeader(group.GroupName, 20);
                categoryHeaderMasked.transform.localScale = Vector3.one * 0.63f;
                categoryHeaderMasked.transform.localPosition = new Vector3(-0.903f, num, -2f);
                num -= 0.63f;
                foreach (IModdedOption option in group.OptionCollection.Options.Values)
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
    }
}
