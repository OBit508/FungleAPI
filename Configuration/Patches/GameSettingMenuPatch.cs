using FungleAPI.Components;
using FungleAPI.PluginLoading;
using FungleAPI.Translation;
using FungleAPI.Utilities;
using FungleAPI.Utilities.Assets;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace FungleAPI.Configuration.Patches
{
    [HarmonyPatch(typeof(GameSettingMenu))]
    internal static class GameSettingMenuPatch
    {
        public static bool TeamConfigTab;
        public static PluginChanger pluginChanger;
        public static PassiveButton TeamConfigButton;
        [HarmonyPatch("Start")]
        [HarmonyPostfix]
        public static void StartPostfix(GameSettingMenu __instance)
        {
            if (GameOptionsManager.Instance.CurrentGameOptions.GameMode != AmongUs.GameOptions.GameModes.HideNSeek && GameOptionsManager.Instance.CurrentGameOptions.GameMode != AmongUs.GameOptions.GameModes.SeekFools)
            {
                TeamConfigTab = false;
                pluginChanger = FungleAssets.PluginChangerPrefab.Instantiate(__instance.ControllerSelectable[0].transform.parent).GetComponent<PluginChanger>();
                pluginChanger.transform.localPosition = new Vector3(-3.36f, 1.67f, -2);
                pluginChanger.OnChange = new Action<ModPlugin>(delegate (ModPlugin plugin)
                {
                    RolesSettingMenuPatch.chanceTabPlugin = null;
                    __instance.GameSettingsTab.Initialize();
                    GamePresetsTabPatch.Update(__instance.PresetsTab);
                });
                __instance.transform.GetChild(2).gameObject.SetActive(false);
                __instance.MenuDescriptionText.transform.parent.localPosition = new Vector3(0, 0.3f, -1);
                __instance.ControllerSelectable[0].transform.localPosition = new Vector3(-2.96f, -0.02f, -2);
                __instance.ControllerSelectable[1].transform.localPosition = new Vector3(-2.96f, -0.62f, -2);
                __instance.ControllerSelectable[2].transform.localPosition = new Vector3(-2.96f, -1.82f, -2);
                TeamConfigButton = GameObject.Instantiate<PassiveButton>(__instance.ControllerSelectable[0].SafeCast<PassiveButton>(), __instance.ControllerSelectable[0].transform.parent);
                TeamConfigButton.transform.localPosition = new Vector3(-2.96f, -1.22f, -2);
                TeamConfigButton.buttonText.GetComponent<TextTranslatorTMP>().enabled = false;
                TeamConfigButton.buttonText.text = FungleTranslation.TeamConfigButtonText.GetString();
                TeamConfigButton.SetNewAction(new Action(delegate
                {
                    __instance.ChangeTab(3, false);
                }));
                __instance.ControllerSelectable.Add(TeamConfigButton);
            }
        }
        [HarmonyPatch("ChangeTab")]
        [HarmonyPrefix]
        public static bool ChangeTabPrefix(GameSettingMenu __instance, [HarmonyArgument(0)] int tabNum, [HarmonyArgument(1)] bool previewOnly)
        {
            if (GameOptionsManager.Instance.CurrentGameOptions.GameMode != AmongUs.GameOptions.GameModes.HideNSeek && GameOptionsManager.Instance.CurrentGameOptions.GameMode != AmongUs.GameOptions.GameModes.SeekFools)
            {
                try
                {
                    if ((previewOnly && Controller.currentTouchType == Controller.TouchType.Joystick) || !previewOnly)
                    {
                        __instance.PresetsTab.gameObject.SetActive(false);
                        __instance.GameSettingsTab.gameObject.SetActive(false);
                        __instance.RoleSettingsTab.gameObject.SetActive(false);
                        __instance.GamePresetsButton.SelectButton(false);
                        __instance.GameSettingsButton.SelectButton(false);
                        __instance.RoleSettingsButton.SelectButton(false);
                        switch (tabNum)
                        {
                            case 0:
                                __instance.PresetsTab.gameObject.SetActive(true);
                                __instance.MenuDescriptionText.text = DestroyableSingleton<TranslationController>.Instance.GetString(StringNames.GamePresetsDescription);
                                break;
                            case 1:
                                TeamConfigTab = false;
                                __instance.GameSettingsTab.Initialize();
                                __instance.GameSettingsTab.gameObject.SetActive(true);
                                __instance.MenuDescriptionText.text = DestroyableSingleton<TranslationController>.Instance.GetString(StringNames.GameSettingsDescription);
                                break;
                            case 2:
                                __instance.RoleSettingsTab.gameObject.SetActive(true);
                                __instance.RoleSettingsTab.OpenMenu(false);
                                __instance.MenuDescriptionText.text = DestroyableSingleton<TranslationController>.Instance.GetString(StringNames.RoleSettingsDescription);
                                break;
                            case 3:
                                TeamConfigTab = true;
                                __instance.GameSettingsTab.Initialize();
                                __instance.GameSettingsTab.gameObject.SetActive(true);
                                __instance.MenuDescriptionText.text = FungleTranslation.TeamConfigDescText.GetString();
                                break;

                        }
                    }
                    if (previewOnly)
                    {
                        __instance.ToggleLeftSideDarkener(false);
                        __instance.ToggleRightSideDarkener(true);
                        return false;
                    }
                    __instance.ToggleLeftSideDarkener(true);
                    __instance.ToggleRightSideDarkener(false);
                    switch (tabNum)
                    {
                        case 0:
                            __instance.PresetsTab.OpenMenu();
                            __instance.GamePresetsButton.SelectButton(true);
                            TeamConfigButton.SelectButton(false);
                            return false;
                        case 1:
                            TeamConfigTab = false;
                            __instance.GameSettingsTab.Initialize();
                            __instance.GameSettingsTab.OpenMenu();
                            __instance.GameSettingsButton.SelectButton(true);
                            TeamConfigButton.SelectButton(false);
                            return false;
                        case 2:
                            __instance.RoleSettingsTab.OpenMenu(true);
                            __instance.RoleSettingsButton.SelectButton(true);
                            TeamConfigButton.SelectButton(false);
                            return false;
                        case 3:
                            TeamConfigTab = true;
                            __instance.GameSettingsTab.Initialize();
                            TeamConfigButton.SelectButton(true);
                            return false;
                        default:
                            return false;
                    }
                }
                catch { return false; }
            }
            return true;
        }
    }
}
