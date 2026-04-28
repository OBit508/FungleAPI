using AmongUs.GameOptions;
using FungleAPI.Components;
using FungleAPI.GameOptions.Patches;
using FungleAPI.Role;
using FungleAPI.Role.Utilities;
using FungleAPI.Teams;
using FungleAPI.Utilities;
using FungleAPI.Utilities.Assets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace FungleAPI.GameOptions.Lobby
{
    public class RoleTab : LobbyTab
    {
        public override string ViewTabButtonText => StringNames.RolesCategory.GetString();
        public override string EditTabButtonText => StringNames.RolesSettings.GetString();
        public override string TabDescriptionText => StringNames.RoleSettingsDescription.GetString();
        public virtual void SetQuotaTab(RolesSettingsMenu rolesSettingsMenu, List<CategoryHeaderEditRole> headerEditRoles)
        {
            float num = 0.662f;
            rolesSettingsMenu.roleTabs = new Il2CppSystem.Collections.Generic.List<PassiveButton>();
            rolesSettingsMenu.roleTabs.Add(rolesSettingsMenu.AllButton);
            Dictionary<ModdedTeam, List<RoleBehaviour>> teams = GameSettingMenuPatch.pluginChanger.CurrentPlugin.GetTeamsAndRoles();
            foreach (KeyValuePair<ModdedTeam, List<RoleBehaviour>> pair in teams)
            {
                List<RoleBehaviour> validRoles = pair.Value;
                validRoles.RemoveAll(r => r.CustomRole() != null && r.CustomRole().Configuration.HideInLobby);
                if (validRoles.Count > 0)
                {
                    CategoryHeaderEditRole categoryHeaderEditRole = pair.Key.CreatCategoryHeaderEditRole(rolesSettingsMenu.RoleChancesSettings.transform);
                    categoryHeaderEditRole.transform.localPosition = new Vector3(4.986f, num, -2f);
                    headerEditRoles.Add(categoryHeaderEditRole);
                    num -= 0.522f;
                    foreach (RoleBehaviour role in validRoles)
                    {
                        if (role.CustomRole() != null)
                        {
                            CreateQuotaOption(rolesSettingsMenu, role.CustomRole(), ref num);
                        }
                    }
                    num -= 0.22f;
                }
            }
        }
        public override void BuildViewTab(LobbyViewSettingsPane lobbyViewSettingsPane)
        {
            float num = 0.95f;
            float num2 = -6.53f;
            float minY = num;
            float lastItemHeight = 0f;
            CategoryHeaderMasked categoryHeaderMasked = GameObject.Instantiate(lobbyViewSettingsPane.categoryHeaderOrigin);
            categoryHeaderMasked.SetHeader(StringNames.RoleQuotaLabel, 61);
            categoryHeaderMasked.transform.SetParent(lobbyViewSettingsPane.settingsContainer);
            categoryHeaderMasked.transform.localScale = Vector3.one;
            categoryHeaderMasked.transform.localPosition = new Vector3(-9.77f, 1.26f, -2f);
            lobbyViewSettingsPane.settingsInfo.Add(categoryHeaderMasked.gameObject);
            Dictionary<ModdedTeam, List<RoleBehaviour>> teams = Plugin.GetTeamsAndRoles();
            List<RoleBehaviour> list = new List<RoleBehaviour>();
            for (int i = 0; i < teams.Count; i++)
            {
                var key = teams.Keys.ToArray()[i];
                List<RoleBehaviour> validRoles = teams[key];
                validRoles.RemoveAll(r => r.CustomRole() != null && r.CustomRole().Configuration.HideInLobby);
                if (validRoles.Count > 0)
                {
                    CategoryHeaderRoleVariant categoryHeaderRoleVariant = key.CreateCategoryHeaderRoleVariant(lobbyViewSettingsPane.settingsContainer);
                    categoryHeaderRoleVariant.transform.localScale = Vector3.one;
                    categoryHeaderRoleVariant.transform.localPosition = new Vector3(0.09f, num, -2f);
                    lobbyViewSettingsPane.settingsInfo.Add(categoryHeaderRoleVariant.gameObject);
                    lastItemHeight = 0.696f;
                    num -= lastItemHeight;
                    minY = Mathf.Min(minY, num);
                    foreach (RoleBehaviour roleBehaviour in validRoles)
                    {
                        if (roleBehaviour.Role != RoleTypes.CrewmateGhost && roleBehaviour.Role != RoleTypes.ImpostorGhost && roleBehaviour.Role != RoleTypes.Crewmate && roleBehaviour.Role != RoleTypes.Impostor && (roleBehaviour.CustomRole() == null || !roleBehaviour.CustomRole().Configuration.HideInLobby))
                        {
                            int chancePerGame = GameOptionsManager.Instance.CurrentGameOptions.RoleOptions.GetChancePerGame(roleBehaviour.Role);
                            int numPerGame = GameOptionsManager.Instance.CurrentGameOptions.RoleOptions.GetNumPerGame(roleBehaviour.Role);
                            bool disabled = numPerGame == 0;
                            ViewSettingsInfoPanelRoleVariant viewPanel = GameObject.Instantiate(lobbyViewSettingsPane.infoPanelRoleOrigin);
                            viewPanel.transform.SetParent(lobbyViewSettingsPane.settingsContainer);
                            viewPanel.transform.localScale = Vector3.one;
                            viewPanel.transform.localPosition = new Vector3(num2, num, -2f);
                            if (!disabled)
                            {
                                list.Add(roleBehaviour);
                            }
                            viewPanel.SetInfo(roleBehaviour.NiceName, numPerGame, chancePerGame, 61, roleBehaviour.CustomRole() == null ? i == 0 ? Palette.CrewmateRoleBlue : Palette.ImpostorRoleRed : roleBehaviour.CustomRole().RoleColor, roleBehaviour.RoleIconSolid, Plugin == FungleAPIPlugin.Plugin ? i == 0 : true, disabled);
                            if (Plugin != FungleAPIPlugin.Plugin)
                            {
                                viewPanel.chanceBackground.color = roleBehaviour.TeamColor.Darken();
                                viewPanel.background.color = viewPanel.chanceBackground.color;
                            }
                            lobbyViewSettingsPane.settingsInfo.Add(viewPanel.gameObject);
                            lastItemHeight = 0.664f;
                            num -= lastItemHeight;
                            minY = Mathf.Min(minY, num);
                        }
                    }
                }
            }
            if (list.Count > 0)
            {
                CategoryHeaderMasked categoryHeaderMasked2 = GameObject.Instantiate(lobbyViewSettingsPane.categoryHeaderOrigin);
                categoryHeaderMasked2.SetHeader(StringNames.RoleSettingsLabel, 61);
                categoryHeaderMasked2.transform.SetParent(lobbyViewSettingsPane.settingsContainer);
                categoryHeaderMasked2.transform.localScale = Vector3.one;
                categoryHeaderMasked2.transform.localPosition = new Vector3(-9.77f, num, -2f);
                lobbyViewSettingsPane.settingsInfo.Add(categoryHeaderMasked2.gameObject);
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
                    AdvancedRoleViewPanel advancedPanel = GameObject.Instantiate(lobbyViewSettingsPane.advancedRolePanelOrigin);
                    advancedPanel.transform.SetParent(lobbyViewSettingsPane.settingsContainer);
                    advancedPanel.transform.localScale = Vector3.one;
                    advancedPanel.transform.localPosition = new Vector3(posX, num, -2f);
                    float height = Plugin == FungleAPIPlugin.Plugin ? advancedPanel.SetUp(list[k], 0.85f, 61) : SetUp(advancedPanel, list[k].CustomRole(), 0.85f, 61);
                    if (height > maxHeightInRow)
                    {
                        maxHeightInRow = height;
                    }
                    lastItemHeight = height;
                    lobbyViewSettingsPane.settingsInfo.Add(advancedPanel.gameObject);
                }
            }
            float contentHeight = Mathf.Abs(minY) + lastItemHeight;
            lobbyViewSettingsPane.scrollBar.SetYBoundsMax(contentHeight);
        }
        public virtual void CreateQuotaOption(RolesSettingsMenu rolesSettingsMenu, ICustomRole customRole, ref float yPos)
        {
            RoleOptionSetting option = UnityEngine.Object.Instantiate(rolesSettingsMenu.roleOptionSettingOrigin, rolesSettingsMenu.RoleChancesSettings.transform);
            option.SetRole(GameOptionsManager.Instance.CurrentGameOptions.RoleOptions, customRole as RoleBehaviour, 20);
            option.OnValueChanged = new Action<OptionBehaviour>(delegate
            {
                customRole.RoleOptions.SetLocal(option.RoleMaxCount, option.RoleChance);
                option.UpdateValuesAndText(GameOptionsManager.Instance.CurrentGameOptions.RoleOptions);
                if (AmongUsClient.Instance.AmHost)
                {
                    SyncManager.RpcSyncRole(customRole as RoleBehaviour);
                }
            });
            option.roleMaxCount = customRole.Configuration.MaxRoleCount;
            option.UpdateValuesAndText(GameOptionsManager.Instance.CurrentGameOptions.RoleOptions);
            option.SetClickMask(rolesSettingsMenu.ButtonClickMask);
            option.transform.localPosition = new Vector3(-0.15f, yPos, -2f);
            option.titleText.text = customRole.RoleName.GetString();
            option.labelSprite.color = customRole.RoleColor;
            option.countText.text = customRole.RoleOptions.LocalRoleCount.ToString();
            option.chanceText.text = customRole.RoleOptions.LocalRoleChance.ToString();
            if (customRole.RoleOptions.Options.Count > 0)
            {
                PassiveButton cog = FungleAssets.CogPrefab.Instantiate(option.transform).GetComponent<PassiveButton>();
                cog.transform.localPosition = new Vector3(-1.278f, -0.3f, 0f);
                cog.ClickMask = rolesSettingsMenu.ButtonClickMask;
                cog.SetNewAction(delegate
                {
                    ChangeTab(rolesSettingsMenu, customRole);
                });
            }
            option.gameObject.SetActive(true);
            rolesSettingsMenu.roleChances.Add(option);
            yPos += -0.43f;
        }
        public virtual void ChangeTab(RolesSettingsMenu rolesSettingsMenu, ICustomRole customRole)
        {
            rolesSettingsMenu.roleDescriptionText.transform.parent.localPosition = new Vector3(2.5176f, -0.2731f, -1f);
            rolesSettingsMenu.roleDescriptionText.transform.parent.localScale = new Vector3(0.0675f, 0.1494f, 0.5687f);
            rolesSettingsMenu.AdvancedRolesSettings.transform.FindChild("InfoLabelBackground").transform.localPosition = new Vector3(1.082f, 0.1054f, -2.5f);
            rolesSettingsMenu.roleScreenshot.sprite = null;
            if (customRole.Configuration.Screenshot != null)
            {
                RenderTexture rt = RenderTexture.GetTemporary(370, 230);
                Graphics.Blit(customRole.Configuration.Screenshot.texture, rt);
                RenderTexture previous = RenderTexture.active;
                RenderTexture.active = rt;
                Texture2D resized = new Texture2D(370, 230);
                resized.ReadPixels(new Rect(0, 0, 370, 230), 0, 0);
                resized.Apply();
                RenderTexture.active = previous;
                RenderTexture.ReleaseTemporary(rt);
                rolesSettingsMenu.roleScreenshot.sprite = Sprite.Create(resized, new Rect(0f, 0f, 370f, 230f), Vector2.one / 2f, 100f);
            }
            Transform transform = rolesSettingsMenu.AdvancedRolesSettings.transform.Find("Background");
            transform.localPosition = new Vector3(1.4041f, -7.08f, 0f);
            transform.GetComponent<SpriteRenderer>().size = new Vector2(89.4628f, 100f);
            for (int i = 0; i < rolesSettingsMenu.advancedSettingChildren.Count; i++)
            {
                GameObject.Destroy(rolesSettingsMenu.advancedSettingChildren[i].gameObject);
            }
            rolesSettingsMenu.ControllerSelectable.Clear();
            rolesSettingsMenu.advancedSettingChildren.Clear();
            rolesSettingsMenu.roleDescriptionText.text = customRole.RoleBlurLong.GetString();
            rolesSettingsMenu.roleTitleText.text = customRole.RoleName.GetString();
            float num = -0.872f;
            rolesSettingsMenu.roleHeaderSprite.color = customRole.RoleColor;
            rolesSettingsMenu.roleHeaderText.color = customRole.RoleColor.Darken(0.3f);
            rolesSettingsMenu.roleHeaderText.text = customRole.RoleName.GetString();
            foreach (IModdedOption config in customRole.RoleOptions.Options.Values)
            {
                OptionBehaviour op = config.CreateOption(rolesSettingsMenu.AdvancedRolesSettings.transform);
                op.SetClickMask(rolesSettingsMenu.ButtonClickMask);
                op.OnValueChanged += new Action<OptionBehaviour>(delegate
                {
                    SyncManager.RpcSyncOption(config);
                });
                op.transform.localPosition = new Vector3(2.17f, num, -2f);
                rolesSettingsMenu.advancedSettingChildren.Add(op);
                num += -0.45f;
            }
            rolesSettingsMenu.scrollBar.CalculateAndSetYBounds(rolesSettingsMenu.AdvancedRolesSettings.transform.GetChildCount(), 1f, 6f, 0.45f);
            rolesSettingsMenu.scrollBar.ScrollToTop();
            rolesSettingsMenu.RoleChancesSettings.SetActive(false);
            rolesSettingsMenu.AdvancedRolesSettings.SetActive(true);
            rolesSettingsMenu.RefreshChildren();
            rolesSettingsMenu.InitializeControllerNavigation();
        }
        public float SetUp(AdvancedRoleViewPanel advancedRoleViewPanel, ICustomRole role, float spacingY, int maskLayer)
        {
            advancedRoleViewPanel.header.SetHeader(role.RoleName, maskLayer);
            advancedRoleViewPanel.header.Background.color = role.RoleColor;
            advancedRoleViewPanel.header.Title.color = role.RoleColor.Darken();
            advancedRoleViewPanel.divider.material.SetInt(PlayerMaterial.MaskLayer, maskLayer);
            float num = advancedRoleViewPanel.yPosStart;
            float num2 = 1.08f;
            for (int i = 0; i < role.RoleOptions.Options.Count; i++)
            {
                IModdedOption baseGameSetting = role.RoleOptions.Options.Values.ElementAt(i);
                ViewSettingsInfoPanel viewSettingsInfoPanel = UnityEngine.Object.Instantiate(advancedRoleViewPanel.infoPanelOrigin);
                viewSettingsInfoPanel.transform.SetParent(advancedRoleViewPanel.transform);
                viewSettingsInfoPanel.transform.localScale = Vector3.one;
                viewSettingsInfoPanel.transform.localPosition = new Vector3(advancedRoleViewPanel.xPosStart, num, -2f);
                if (baseGameSetting.Data.Type == OptionTypes.Checkbox)
                {
                    viewSettingsInfoPanel.SetInfoCheckbox(baseGameSetting.Data.Title, maskLayer, bool.Parse(baseGameSetting.GetStringValue(AmongUsClient.Instance.AmHost)));
                }
                else
                {
                    viewSettingsInfoPanel.SetInfo(baseGameSetting.Data.Title, baseGameSetting.GetStringValue(AmongUsClient.Instance.AmHost), maskLayer);
                }
                num -= spacingY;
                if (i > 0)
                {
                    num2 += 0.8f;
                }
            }
            return num2;
        }
        public override void BuildEditTab(GameOptionsMenu gameOptionsMenu)
        {
            throw new NotImplementedException();
        }
    }
}
