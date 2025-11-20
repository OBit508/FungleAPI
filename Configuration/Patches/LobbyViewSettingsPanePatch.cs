using AmongUs.GameOptions;
using FungleAPI.Components;
using FungleAPI.Configuration.Attributes;
using FungleAPI.PluginLoading;
using FungleAPI.Role;
using FungleAPI.Role.Teams;
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

namespace FungleAPI.Configuration.Patches
{
    [HarmonyPatch(typeof(LobbyViewSettingsPane))]
    internal static class LobbyViewSettingsPanePatch
    {
        
        public static bool TeamsTab;
        public static PassiveButton TeamsTabButton;
        public static PluginChanger pluginChanger;
        [HarmonyPatch("Awake")]
        [HarmonyPostfix]
        public static void AwakePostfix(LobbyViewSettingsPane __instance)
        {
            if (GameOptionsManager.Instance.CurrentGameOptions.GameMode != AmongUs.GameOptions.GameModes.HideNSeek && GameOptionsManager.Instance.CurrentGameOptions.GameMode != AmongUs.GameOptions.GameModes.SeekFools)
            {
                TeamsTab = false;
                __instance.gameModeText.gameObject.SetActive(false);
                pluginChanger = FungleAssets.PluginChangerPrefab.Instantiate(__instance.rolesTabButton.transform.parent).GetComponent<PluginChanger>();
                pluginChanger.transform.localPosition = __instance.gameModeText.transform.localPosition;
                pluginChanger.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
                pluginChanger.OnChange = new Action<ModPlugin>(delegate
                {
                    if (TeamsTab)
                    {
                        for (int i = 0; i < __instance.settingsInfo.Count; i++)
                        {
                            GameObject.Destroy(__instance.settingsInfo[i].gameObject);
                        }
                        __instance.settingsInfo.Clear();
                        DrawTeamsTab(__instance);
                    }
                    else
                    {
                        __instance.RefreshTab();
                    }
                    __instance.scrollBar.ScrollToTop();
                });
                TeamsTabButton = GameObject.Instantiate<PassiveButton>(__instance.rolesTabButton, __instance.rolesTabButton.transform.parent);
                TeamsTabButton.transform.localPosition = new Vector3(2.071f, 1.404f, 0);
                TeamsTabButton.buttonText.GetComponent<TextTranslatorTMP>().enabled = false;
                TeamsTabButton.buttonText.text = FungleTranslation.TeamsText.GetString();
                TeamsTabButton.SetNewAction(new Action(delegate
                {
                    for (int i = 0; i < __instance.settingsInfo.Count; i++)
                    {
                        GameObject.Destroy(__instance.settingsInfo[i].gameObject);
                    }
                    __instance.settingsInfo.Clear();
                    __instance.rolesTabButton.SelectButton(false);
                    __instance.taskTabButton.SelectButton(false);
                    TeamsTabButton.SelectButton(true);
                    TeamsTab = true;
                    DrawTeamsTab(__instance);
                }));
            }
        }
        [HarmonyPatch("DrawNormalTab")]
        [HarmonyPrefix]
        public static bool DrawNormalTabPrefix(LobbyViewSettingsPane __instance)
        {
            if (GameOptionsManager.Instance.CurrentGameOptions.GameMode != AmongUs.GameOptions.GameModes.HideNSeek && GameOptionsManager.Instance.CurrentGameOptions.GameMode != AmongUs.GameOptions.GameModes.SeekFools)
            {
                TeamsTabButton.SelectButton(false);
                TeamsTab = false;
                if (pluginChanger.CurrentPlugin != FungleAPIPlugin.Plugin)
                {
                    DrawNormalTab(__instance);
                    return false;
                }
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
                        list.RemoveAt(0);
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
                return false;
            }
            return true;
        }
        [HarmonyPatch("DrawRolesTab")]
        [HarmonyPrefix]
        public static bool DrawRolesTabPrefix(LobbyViewSettingsPane __instance)
        {
            if (GameOptionsManager.Instance.CurrentGameOptions.GameMode != AmongUs.GameOptions.GameModes.HideNSeek && GameOptionsManager.Instance.CurrentGameOptions.GameMode != AmongUs.GameOptions.GameModes.SeekFools)
            {
                TeamsTabButton.SelectButton(false);
                TeamsTab = false;
                float num = 0.95f;
                float num2 = -6.53f;
                float minY = num;
                float lastItemHeight = 0f;
                CategoryHeaderMasked categoryHeaderMasked = UnityEngine.Object.Instantiate(__instance.categoryHeaderOrigin);
                categoryHeaderMasked.SetHeader(StringNames.RoleQuotaLabel, 61);
                categoryHeaderMasked.transform.SetParent(__instance.settingsContainer);
                categoryHeaderMasked.transform.localScale = Vector3.one;
                categoryHeaderMasked.transform.localPosition = new Vector3(-9.77f, 1.26f, -2f);
                __instance.settingsInfo.Add(categoryHeaderMasked.gameObject);
                Dictionary<ModdedTeam, List<RoleBehaviour>> teams = pluginChanger.CurrentPlugin.GetTeamsAndRoles();
                List<RoleBehaviour> list = new List<RoleBehaviour>();
                for (int i = 0; i < teams.Count; i++)
                {
                    List<RoleBehaviour> validRoles = teams[teams.Keys.ToArray()[i]];
                    validRoles.RemoveAll(r => r.CustomRole() != null && r.CustomRole().HideRole);
                    if (validRoles.Count > 0)
                    {
                        CategoryHeaderRoleVariant categoryHeaderRoleVariant = teams.Keys.ToArray()[i].CreateCategoryHeaderRoleVariant(__instance.settingsContainer);
                        categoryHeaderRoleVariant.transform.localScale = Vector3.one;
                        categoryHeaderRoleVariant.transform.localPosition = new Vector3(0.09f, num, -2f);
                        __instance.settingsInfo.Add(categoryHeaderRoleVariant.gameObject);
                        lastItemHeight = 0.696f;
                        num -= lastItemHeight;
                        minY = Mathf.Min(minY, num);
                        foreach (RoleBehaviour roleBehaviour in validRoles)
                        {
                            if (roleBehaviour.Role != RoleTypes.CrewmateGhost && roleBehaviour.Role != RoleTypes.ImpostorGhost && roleBehaviour.Role != RoleTypes.Crewmate && roleBehaviour.Role != RoleTypes.Impostor && (roleBehaviour.CustomRole() == null || roleBehaviour.CustomRole() != null && !roleBehaviour.CustomRole().HideRole))
                            {
                                int chancePerGame = GameOptionsManager.Instance.CurrentGameOptions.RoleOptions.GetChancePerGame(roleBehaviour.Role);
                                int numPerGame = GameOptionsManager.Instance.CurrentGameOptions.RoleOptions.GetNumPerGame(roleBehaviour.Role);
                                bool disabled = numPerGame == 0;
                                ViewSettingsInfoPanelRoleVariant viewPanel = UnityEngine.Object.Instantiate(__instance.infoPanelRoleOrigin);
                                viewPanel.transform.SetParent(__instance.settingsContainer);
                                viewPanel.transform.localScale = Vector3.one;
                                viewPanel.transform.localPosition = new Vector3(num2, num, -2f);
                                if (!disabled)
                                {
                                    list.Add(roleBehaviour);
                                }
                                viewPanel.SetInfo(
                                    roleBehaviour.NiceName,
                                    numPerGame,
                                    chancePerGame,
                                    61,
                                    roleBehaviour.CustomRole() == null
                                        ? i == 0 ? Palette.CrewmateRoleBlue : Palette.ImpostorRoleRed
                                        : roleBehaviour.CustomRole().RoleColor,
                                    roleBehaviour.RoleIconSolid,
                                    pluginChanger.CurrentPlugin == FungleAPIPlugin.Plugin ? i == 0 : true,
                                    disabled
                                );
                                if (pluginChanger.CurrentPlugin != FungleAPIPlugin.Plugin)
                                {
                                    viewPanel.chanceBackground.color = Utilities.Helpers.Dark(roleBehaviour.TeamColor);
                                    viewPanel.background.color = viewPanel.chanceBackground.color;
                                }
                                __instance.settingsInfo.Add(viewPanel.gameObject);
                                lastItemHeight = 0.664f;
                                num -= lastItemHeight;
                                minY = Mathf.Min(minY, num);
                            }
                        }
                    }
                }
                if (list.Count > 0)
                {
                    CategoryHeaderMasked categoryHeaderMasked2 = UnityEngine.Object.Instantiate(__instance.categoryHeaderOrigin);
                    categoryHeaderMasked2.SetHeader(StringNames.RoleSettingsLabel, 61);
                    categoryHeaderMasked2.transform.SetParent(__instance.settingsContainer);
                    categoryHeaderMasked2.transform.localScale = Vector3.one;
                    categoryHeaderMasked2.transform.localPosition = new Vector3(-9.77f, num, -2f);
                    __instance.settingsInfo.Add(categoryHeaderMasked2.gameObject);
                    lastItemHeight = 2.1f;
                    num -= lastItemHeight;
                    minY = Mathf.Min(minY, num);
                    float maxHeightInRow = 0f;
                    for (int k = 0; k < list.Count; k++)
                    {
                        float posX;
                        if (k % 2 == 0)
                        {
                            posX = -5.8f;
                            if (k > 0)
                            {
                                num -= maxHeightInRow + 0.85f;
                                minY = Mathf.Min(minY, num);
                                maxHeightInRow = 0f;
                            }
                        }
                        else
                        {
                            posX = 0.14999962f;
                        }
                        AdvancedRoleViewPanel advancedPanel = UnityEngine.Object.Instantiate(__instance.advancedRolePanelOrigin);
                        advancedPanel.transform.SetParent(__instance.settingsContainer);
                        advancedPanel.transform.localScale = Vector3.one;
                        advancedPanel.transform.localPosition = new Vector3(posX, num, -2f);
                        float height = pluginChanger.CurrentPlugin == FungleAPIPlugin.Plugin
                            ? advancedPanel.SetUp(list[k], 0.85f, 61)
                            : advancedPanel.SetUp(list[k].CustomRole(), 0.85f, 61);
                        if (height > maxHeightInRow)
                        {
                            maxHeightInRow = height;
                        }
                        lastItemHeight = height;
                        __instance.settingsInfo.Add(advancedPanel.gameObject);
                    }
                }
                float contentHeight = Mathf.Abs(minY) + lastItemHeight;
                __instance.scrollBar.SetYBoundsMax(contentHeight);
                return false;
            }
            return true;
        }
        public static float SetUp(this AdvancedRoleViewPanel advancedRoleViewPanel, ICustomRole role, float spacingY, int maskLayer)
        {
            advancedRoleViewPanel.header.SetHeader(role.RoleName, maskLayer);
            advancedRoleViewPanel.header.Background.color = role.RoleColor;
            advancedRoleViewPanel.header.Title.color = Utilities.Helpers.Dark(role.RoleColor);
            advancedRoleViewPanel.divider.material.SetInt(PlayerMaterial.MaskLayer, maskLayer);
            float num = advancedRoleViewPanel.yPosStart;
            float num2 = 1.08f;
            for (int i = 0; i < role.Options.Count; i++)
            {
                ModdedOption baseGameSetting = role.Options[i];
                ViewSettingsInfoPanel viewSettingsInfoPanel = UnityEngine.Object.Instantiate(advancedRoleViewPanel.infoPanelOrigin);
                viewSettingsInfoPanel.transform.SetParent(advancedRoleViewPanel.transform);
                viewSettingsInfoPanel.transform.localScale = Vector3.one;
                viewSettingsInfoPanel.transform.localPosition = new Vector3(advancedRoleViewPanel.xPosStart, num, -2f);
                if (baseGameSetting.Data.Type == OptionTypes.Checkbox)
                {
                    viewSettingsInfoPanel.SetInfoCheckbox(baseGameSetting.Data.Title, maskLayer, bool.Parse(baseGameSetting.GetValue()));
                }
                else
                {
                    viewSettingsInfoPanel.SetInfo(baseGameSetting.Data.Title, baseGameSetting.GetValue(), maskLayer);
                }
                num -= spacingY;
                if (i > 0)
                {
                    num2 += 0.8f;
                }
            }
            return num2;
        }
        public static void DrawNormalTab(LobbyViewSettingsPane menu)
        {
            float num = 1.44f;
            foreach (SettingsGroup group in pluginChanger.CurrentPlugin.Settings.Groups)
            {
                CategoryHeaderMasked categoryHeaderMasked = UnityEngine.Object.Instantiate(menu.categoryHeaderOrigin);
                categoryHeaderMasked.SetHeader(group.GroupName, 61);
                categoryHeaderMasked.transform.SetParent(menu.settingsContainer);
                categoryHeaderMasked.transform.localScale = Vector3.one;
                categoryHeaderMasked.transform.localPosition = new Vector3(-9.77f, num, -2f);
                menu.settingsInfo.Add(categoryHeaderMasked.gameObject);
                num -= 1.05f;
                for (int i = 0; i < group.Options.Count; i++)
                {
                    ViewSettingsInfoPanel viewSettingsInfoPanel = UnityEngine.Object.Instantiate(menu.infoPanelOrigin);
                    viewSettingsInfoPanel.transform.SetParent(menu.settingsContainer);
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
                    if (group.Options[i].Data.Type == OptionTypes.Checkbox)
                    {
                        viewSettingsInfoPanel.SetInfoCheckbox(group.Options[i].Data.Title, 61, bool.Parse(group.Options[i].GetValue()));
                    }
                    else
                    {
                        viewSettingsInfoPanel.SetInfo(group.Options[i].Data.Title, group.Options[i].GetValue(), 61);
                    }
                    menu.settingsInfo.Add(viewSettingsInfoPanel.gameObject);
                }
                num -= 0.85f;
            }
            menu.scrollBar.CalculateAndSetYBounds(menu.settingsInfo.Count + 10, 2f, 6f, 0.85f);
        }
        public static void DrawTeamsTab(LobbyViewSettingsPane menu)
        {
            menu.scrollBar.ScrollToTop();
            float num = 1.44f;
            foreach (ModdedTeam group in pluginChanger.CurrentPlugin.Teams)
            {
                if (group != ModdedTeam.Crewmates)
                {
                    CategoryHeaderMasked categoryHeaderMasked = UnityEngine.Object.Instantiate(menu.categoryHeaderOrigin);
                    categoryHeaderMasked.SetHeader(group.TeamName, 61);
                    categoryHeaderMasked.transform.SetParent(menu.settingsContainer);
                    categoryHeaderMasked.transform.localScale = Vector3.one;
                    categoryHeaderMasked.transform.localPosition = new Vector3(-9.77f, num, -2f);
                    menu.settingsInfo.Add(categoryHeaderMasked.gameObject);
                    num -= 1.05f;
                    for (int i = 0; i < group.ExtraConfigs.Count + 2; i++)
                    {
                        ViewSettingsInfoPanel viewSettingsInfoPanel = UnityEngine.Object.Instantiate(menu.infoPanelOrigin);
                        viewSettingsInfoPanel.transform.SetParent(menu.settingsContainer);
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
                            if (group.ExtraConfigs[i - 2].Data.Type == OptionTypes.Checkbox)
                            {
                                viewSettingsInfoPanel.SetInfoCheckbox(group.ExtraConfigs[i - 2].Data.Title, 61, bool.Parse(group.ExtraConfigs[i - 2].GetValue()));
                            }
                            else
                            {
                                viewSettingsInfoPanel.SetInfo(group.ExtraConfigs[i - 2].Data.Title, group.ExtraConfigs[i - 2].GetValue(), 61);
                            }
                        }
                        else if (i == 0)
                        {
                            viewSettingsInfoPanel.SetInfo(group.CountData.Title, group.CountAndPriority.GetCount().ToString(), 61);
                        }
                        else if (i == 1)
                        {
                            viewSettingsInfoPanel.SetInfo(group.PriorityData.Title, group.CountAndPriority.GetPriority().ToString(), 61);
                        }
                        menu.settingsInfo.Add(viewSettingsInfoPanel.gameObject);
                    }
                    num -= 0.85f;
                }
            }
            menu.scrollBar.CalculateAndSetYBounds(menu.settingsInfo.Count + 10, 2f, 6f, 0.85f);
        }
    }
}
