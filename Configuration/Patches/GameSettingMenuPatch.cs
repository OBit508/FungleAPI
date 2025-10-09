using FungleAPI.Components;
using FungleAPI.Translation;
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
    [HarmonyPatch(typeof(GameSettingMenu))]
    internal static class GameSettingMenuPatch
    {
        public static bool TeamConfigTab;
        public static PluginChanger pluginChanger;
        public static PassiveButton TeamConfigButton;
        public static Translator teamConfigButton;
        public static Translator teamConfigDesc;
        public static StringNames TeamConfigButtonText
        {
            get
            {
                if (teamConfigButton == null)
                {
                    teamConfigButton = new Translator("Teams Configurations");
                    teamConfigButton.AddTranslation(SupportedLangs.Latam, "Configuraciones de equipos");
                    teamConfigButton.AddTranslation(SupportedLangs.Brazilian, "Configurações dos Times");
                    teamConfigButton.AddTranslation(SupportedLangs.Portuguese, "Configurações das Equipes");
                    teamConfigButton.AddTranslation(SupportedLangs.Korean, "팀 설정");
                    teamConfigButton.AddTranslation(SupportedLangs.Russian, "Настройки команд");
                    teamConfigButton.AddTranslation(SupportedLangs.Dutch, "Teamconfiguraties");
                    teamConfigButton.AddTranslation(SupportedLangs.Filipino, "Mga configuration ng koponan");
                    teamConfigButton.AddTranslation(SupportedLangs.French, "Configurations des équipes");
                    teamConfigButton.AddTranslation(SupportedLangs.German, "Teamkonfigurationen");
                    teamConfigButton.AddTranslation(SupportedLangs.Italian, "Configurazioni delle squadre");
                    teamConfigButton.AddTranslation(SupportedLangs.Japanese, "チーム設定");
                    teamConfigButton.AddTranslation(SupportedLangs.Spanish, "Configuraciones de equipos");
                    teamConfigButton.AddTranslation(SupportedLangs.SChinese, "队伍配置");
                    teamConfigButton.AddTranslation(SupportedLangs.TChinese, "隊伍配置");
                    teamConfigButton.AddTranslation(SupportedLangs.Irish, "Cumraíochtaí Foirne");
                }
                return teamConfigButton.StringName;
            }
        }
        public static StringNames TeamConfigDescText
        {
            get
            {
                if (teamConfigDesc == null)
                {
                    teamConfigDesc = new Translator("Edit the Teams settings for your lobby.");
                    teamConfigDesc.AddTranslation(SupportedLangs.Latam, "Edita la configuración de equipos para tu sala.");
                    teamConfigDesc.AddTranslation(SupportedLangs.Brazilian, "Edite as configurações de Times para sua sala.");
                    teamConfigDesc.AddTranslation(SupportedLangs.Portuguese, "Edite as configurações das Equipes para sua sala.");
                    teamConfigDesc.AddTranslation(SupportedLangs.Korean, "로비의 팀 설정을 편집하세요.");
                    teamConfigDesc.AddTranslation(SupportedLangs.Russian, "Измените настройки команд для вашей комнаты.");
                    teamConfigDesc.AddTranslation(SupportedLangs.Dutch, "Bewerk de teaminstellingen voor je lobby.");
                    teamConfigDesc.AddTranslation(SupportedLangs.Filipino, "I-edit ang mga setting ng koponan para sa iyong lobby.");
                    teamConfigDesc.AddTranslation(SupportedLangs.French, "Modifiez les paramètres des équipes pour votre salon.");
                    teamConfigDesc.AddTranslation(SupportedLangs.German, "Bearbeiten Sie die Team-Einstellungen für Ihre Lobby.");
                    teamConfigDesc.AddTranslation(SupportedLangs.Italian, "Modifica le impostazioni delle squadre per la tua lobby.");
                    teamConfigDesc.AddTranslation(SupportedLangs.Japanese, "ロビーのチーム設定を編集してください。");
                    teamConfigDesc.AddTranslation(SupportedLangs.Spanish, "Edita la configuración de equipos para tu sala.");
                    teamConfigDesc.AddTranslation(SupportedLangs.SChinese, "编辑您房间的队伍设置。");
                    teamConfigDesc.AddTranslation(SupportedLangs.TChinese, "編輯您房間的隊伍設定。");
                    teamConfigDesc.AddTranslation(SupportedLangs.Irish, "Cuir eagarthóireacht ar na socruithe foirne do do lóiste.");
                }
                return teamConfigDesc.StringName;
            }
        }
        [HarmonyPatch("Start")]
        [HarmonyPostfix]
        public static void StartPostfix(GameSettingMenu __instance)
        {
            TeamConfigTab = false;
            pluginChanger = FungleAPIPlugin.PluginChangerPrefab.Instantiate(__instance.ControllerSelectable[0].transform.parent);
            pluginChanger.transform.localPosition = new Vector3(-3.36f, 1.67f, -2);
            pluginChanger.OnChange = new Action<ModPlugin>(delegate (ModPlugin plugin)
            {
                RolesSettingMenuPatch.chanceTabPlugin = null;
                __instance.GameSettingsTab.Initialize();
            });
            pluginChanger.Initialize();
            __instance.transform.GetChild(2).gameObject.SetActive(false);
            __instance.MenuDescriptionText.transform.parent.localPosition = new Vector3(0, 0.3f, -1);
            __instance.ControllerSelectable[0].transform.localPosition = new Vector3(-2.96f, -0.02f, -2);
            __instance.ControllerSelectable[1].transform.localPosition = new Vector3(-2.96f, -0.62f, -2);
            __instance.ControllerSelectable[2].transform.localPosition = new Vector3(-2.96f, -1.82f, -2);
            TeamConfigButton = GameObject.Instantiate<PassiveButton>(__instance.ControllerSelectable[0].SafeCast<PassiveButton>(), __instance.ControllerSelectable[0].transform.parent);
            TeamConfigButton.transform.localPosition = new Vector3(-2.96f, -1.22f, -2);
            TeamConfigButton.buttonText.GetComponent<TextTranslatorTMP>().enabled = false;
            TeamConfigButton.buttonText.text = TeamConfigButtonText.GetString();
            TeamConfigButton.SetNewAction(new Action(delegate
            {
                __instance.ChangeTab(3, false);
            }));
            __instance.ControllerSelectable.Add(TeamConfigButton);

        }
        [HarmonyPatch("ChangeTab")]
        [HarmonyPrefix]
        public static bool ChangeTabPrefix(GameSettingMenu __instance, [HarmonyArgument(0)] int tabNum, [HarmonyArgument(1)] bool previewOnly)
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
                            __instance.MenuDescriptionText.text = TeamConfigDescText.GetString();
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
            } catch { return false; }
        }
    }
}
