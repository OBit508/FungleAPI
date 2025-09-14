using AmongUs.GameOptions;
using FungleAPI.Configuration.Attributes;
using FungleAPI.Role;
using FungleAPI.Role.Teams;
using FungleAPI.Translation;
using FungleAPI.Utilities;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using static Il2CppMono.Security.X509.X520;
using static UnityEngine.GraphicsBuffer;

namespace FungleAPI.Patches
{
    [HarmonyPatch(typeof(LobbyViewSettingsPane))]
    internal static class LobbyViewSettingsPanePatch
    {
        public static ModPlugin currentPlugin;
        public static int currentIndex;
        public static PassiveButton SwitchButton;
        [HarmonyPatch("Awake")]
        [HarmonyPostfix]
        public static void AwakePostfix(LobbyViewSettingsPane __instance)
        {
            currentPlugin = FungleAPIPlugin.Plugin;
            currentIndex = 0;
            SwitchButton = GameObject.Instantiate(__instance.rolesTabButton, __instance.rolesTabButton.transform.parent);
            SwitchButton.transform.localPosition = new Vector3(3.6f, 1.404f, 0);
            SwitchButton.OnClick = new UnityEngine.UI.Button.ButtonClickedEvent();
            SwitchButton.OnClick.AddListener(new Action(delegate
            {
                if (currentIndex + 1 >= ModPlugin.AllPlugins.Count)
                {
                    currentIndex = 0;
                }
                else
                {
                    currentIndex++;
                }
                currentPlugin = ModPlugin.AllPlugins[currentIndex];
                SwitchButton.transform.GetChild(0).GetChild(0).GetComponent<TextMeshPro>().text = currentPlugin.ModName;
                __instance.RefreshTab();
                __instance.scrollBar.ScrollToTop();
            }));
            SwitchButton.transform.GetChild(0).GetChild(0).GetComponent<TextTranslatorTMP>().enabled = false;
            SwitchButton.transform.GetChild(0).GetChild(0).GetComponent<TextMeshPro>().text = currentPlugin.ModName;
        }
        [HarmonyPatch("DrawNormalTab")]
        [HarmonyPrefix]
        public static bool DrawNormalTabPrefix(LobbyViewSettingsPane __instance)
        {
            if (currentPlugin != FungleAPIPlugin.Plugin)
            {
                DrawNormalTab(__instance);
                return false;
            }
            return true;
        }
        [HarmonyPatch("DrawRolesTab")]
        [HarmonyPrefix]
        public static bool DrawRolesTabPrefix(LobbyViewSettingsPane __instance)
        {
            float num = 0.95f;
            float num2 = -6.53f;
            float minY = num;
            float lastItemHeight = 0f;
            CategoryHeaderMasked categoryHeaderMasked = GameObject.Instantiate(__instance.categoryHeaderOrigin);
            categoryHeaderMasked.SetHeader(StringNames.RoleQuotaLabel, 61);
            categoryHeaderMasked.transform.SetParent(__instance.settingsContainer);
            categoryHeaderMasked.transform.localScale = Vector3.one;
            categoryHeaderMasked.transform.localPosition = new Vector3(-9.77f, 1.26f, -2f);
            __instance.settingsInfo.Add(categoryHeaderMasked.gameObject);
            List<ModdedTeam> UsedTeams = new List<ModdedTeam>();
            foreach (RoleBehaviour role in currentPlugin.Roles)
            {
                if (!UsedTeams.Contains(role.GetTeam()) &&
                    (role.CustomRole() == null || role.CustomRole() != null && !role.CustomRole().Configuration.HideRole) &&
                    (currentPlugin == FungleAPIPlugin.Plugin && role.GetTeam() != ModdedTeam.Neutrals ||
                     currentPlugin != FungleAPIPlugin.Plugin))
                {
                    UsedTeams.Add(role.GetTeam());
                }
            }
            List<RoleBehaviour> list = new List<RoleBehaviour>();
            for (int i = 0; i < UsedTeams.Count; i++)
            {
                CategoryHeaderRoleVariant categoryHeaderRoleVariant = GameObject.Instantiate(__instance.categoryHeaderRoleOrigin);
                categoryHeaderRoleVariant.SetHeader(
                    i == 0 ? StringNames.CrewmateRolesHeader : StringNames.ImpostorRolesHeader,
                    61
                );
                if (currentPlugin != FungleAPIPlugin.Plugin)
                {
                    string[] names = StringNames.CrewmateRolesHeader.GetString().Split(" ");
                    categoryHeaderRoleVariant.Title.text = names[0] + " " + names[1] + " " + UsedTeams[i].TeamName.GetString();
                    categoryHeaderRoleVariant.Background.color = UsedTeams[i].TeamHeaderColor;
                    categoryHeaderRoleVariant.Title.color = Helpers.Dark(categoryHeaderRoleVariant.Background.color);
                }
                categoryHeaderRoleVariant.transform.SetParent(__instance.settingsContainer);
                categoryHeaderRoleVariant.transform.localScale = Vector3.one;
                categoryHeaderRoleVariant.transform.localPosition = new Vector3(0.09f, num, -2f);
                __instance.settingsInfo.Add(categoryHeaderRoleVariant.gameObject);
                lastItemHeight = 0.696f;
                num -= lastItemHeight;
                minY = Mathf.Min(minY, num);
                foreach (RoleBehaviour roleBehaviour in currentPlugin.Roles)
                {
                    if (roleBehaviour.GetTeam() == UsedTeams[i] &&
                        roleBehaviour.Role != RoleTypes.CrewmateGhost &&
                        roleBehaviour.Role != RoleTypes.ImpostorGhost &&
                        roleBehaviour.Role != RoleTypes.Crewmate &&
                        roleBehaviour.Role != RoleTypes.Impostor &&
                        roleBehaviour.Role != CustomRoleManager.NeutralGhost.Role)
                    {
                        int chancePerGame = GameOptionsManager.Instance.CurrentGameOptions.RoleOptions.GetChancePerGame(roleBehaviour.Role);
                        int numPerGame = GameOptionsManager.Instance.CurrentGameOptions.RoleOptions.GetNumPerGame(roleBehaviour.Role);
                        bool disabled = numPerGame == 0;
                        ViewSettingsInfoPanelRoleVariant viewPanel = GameObject.Instantiate(__instance.infoPanelRoleOrigin);
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
                            currentPlugin == FungleAPIPlugin.Plugin ? i == 0 : true,
                            disabled
                        );
                        if (currentPlugin != FungleAPIPlugin.Plugin)
                        {
                            viewPanel.chanceBackground.color = Helpers.Dark(roleBehaviour.TeamColor);
                            viewPanel.background.color = viewPanel.chanceBackground.color;
                        }
                        __instance.settingsInfo.Add(viewPanel.gameObject);
                        lastItemHeight = 0.664f;
                        num -= lastItemHeight;
                        minY = Mathf.Min(minY, num);
                    }
                }
            }
            if (list.Count > 0)
            {
                CategoryHeaderMasked categoryHeaderMasked2 = GameObject.Instantiate(__instance.categoryHeaderOrigin);
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
                    AdvancedRoleViewPanel advancedPanel = GameObject.Instantiate(__instance.advancedRolePanelOrigin);
                    advancedPanel.transform.SetParent(__instance.settingsContainer);
                    advancedPanel.transform.localScale = Vector3.one;
                    advancedPanel.transform.localPosition = new Vector3(posX, num, -2f);
                    float height = currentPlugin == FungleAPIPlugin.Plugin
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
        public static float SetUp(this AdvancedRoleViewPanel advancedRoleViewPanel, ICustomRole role, float spacingY, int maskLayer)
        {
            advancedRoleViewPanel.header.SetHeader(role.RoleName, maskLayer);
            advancedRoleViewPanel.header.Background.color = role.RoleColor;
            advancedRoleViewPanel.header.Title.color = Helpers.Dark(role.RoleColor);
            advancedRoleViewPanel.divider.material.SetInt(PlayerMaterial.MaskLayer, maskLayer);
            float num = advancedRoleViewPanel.yPosStart;
            float num2 = 1.08f;
            for (int i = 0; i < role.Configuration.Configs.Count; i++)
            {
                ModdedOption baseGameSetting = role.Configuration.Configs[i];
                ViewSettingsInfoPanel viewSettingsInfoPanel = GameObject.Instantiate(advancedRoleViewPanel.infoPanelOrigin);
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
            foreach (SettingsGroup group in currentPlugin.Settings.Groups)
            {
                CategoryHeaderMasked categoryHeaderMasked = GameObject.Instantiate<CategoryHeaderMasked>(menu.categoryHeaderOrigin);
                categoryHeaderMasked.SetHeader(group.GroupName, 61);
                categoryHeaderMasked.transform.SetParent(menu.settingsContainer);
                categoryHeaderMasked.transform.localScale = Vector3.one;
                categoryHeaderMasked.transform.localPosition = new Vector3(-9.77f, num, -2f);
                menu.settingsInfo.Add(categoryHeaderMasked.gameObject);
                num -= 1.05f;
                for (int i = 0; i < group.Options.Count; i++)
                {
                    ViewSettingsInfoPanel viewSettingsInfoPanel = GameObject.Instantiate<ViewSettingsInfoPanel>(menu.infoPanelOrigin);
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
            menu.scrollBar.CalculateAndSetYBounds((float)(menu.settingsInfo.Count + 10), 2f, 6f, 0.85f);
        }
    }
}
