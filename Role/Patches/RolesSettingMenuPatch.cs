using AmongUs.GameOptions;
using Discord;
using Epic.OnlineServices;
using FungleAPI.Assets;
using FungleAPI.Configuration;
using FungleAPI.MonoBehaviours;
using FungleAPI.Role.Teams;
using FungleAPI.Roles;
using FungleAPI.Rpc;
using FungleAPI.Utilities;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.ProBuilder;
using static Rewired.Platforms.Custom.CustomPlatformUnifiedKeyboardSource.KeyPropertyMap;
using static Rewired.UI.ControlMapper.ControlMapper;

namespace FungleAPI.Role.Patches
{
    [HarmonyPatch(typeof(RolesSettingsMenu))]
    internal class RolesSettingMenuPatch
    {
        public static ModPlugin currentPlugin;
        public static ModPlugin chanceTabPlugin;
        public static ToggleOption togglePrefab;
        public static NumberOption numberPrefab;
        public static SpriteRenderer labelSprite;
        public static TextMeshPro labelText;
        public static GameObject AdvancedTab;
        public static List<OptionBehaviour> options = new List<OptionBehaviour>();
        public static PassiveButton SwitchButton;
        public static int currentIndex;
        public static void Reset()
        {
            currentPlugin = FungleAPIPlugin.Plugin;
            chanceTabPlugin = null;
            options.Clear();
        }
        public static void SetBoundsY(RolesSettingsMenu menu)
        {
            menu.scrollBar.ContentYBounds.min = currentPlugin == FungleAPIPlugin.Plugin ? 0 : 0.7f;
            int count = 0;
            for (int i = 0; i < menu.RoleChancesSettings.transform.GetChildCount(); i++)
            {
                if (menu.RoleChancesSettings.transform.GetChild(i).gameObject.active)
                {
                    count++;
                }
            }
            menu.scrollBar.CalculateAndSetYBounds(count + (currentPlugin == FungleAPIPlugin.Plugin ? 3 : 0), 1f, 6f, 0.43f);
        }
        [HarmonyPatch("Update")]
        [HarmonyPrefix]
        public static void OnUpdate(RolesSettingsMenu __instance)
        {
            if (SwitchButton == null)
            {
                Reset();
                currentIndex = 0;
                currentPlugin = FungleAPIPlugin.Plugin;
                SwitchButton = UnityEngine.Object.Instantiate(__instance.transform.parent.parent.GetChild(4).GetChild(0).GetComponent<PassiveButton>(), __instance.transform.parent.parent.GetChild(4));
                SwitchButton.transform.localPosition = new Vector3(-2.96f, 1.57f, -2);
                SwitchButton.OnClick = new UnityEngine.UI.Button.ButtonClickedEvent();
                SwitchButton.OnClick.AddListener(new Action(delegate
                {
                    Reset();
                    if (currentIndex + 1 >= ModPlugin.AllPlugins.Count)
                    {
                        currentIndex = 0;
                    }
                    else
                    {
                        currentIndex++;
                        currentPlugin = ModPlugin.AllPlugins[currentIndex];
                    }
                }));
                SwitchButton.gameObject.AddComponent<Updater>().onUpdate = new Action(delegate
                {
                    __instance.transform.parent.parent.GetChild(2).gameObject.SetActive(!__instance.RoleChancesSettings.active);
                    SwitchButton.gameObject.SetActive(__instance.RoleChancesSettings.active);
                    SwitchButton.transform.GetChild(0).GetChild(0).GetComponent<TextMeshPro>().text = currentPlugin.ModName;
                });
            }
            if (togglePrefab == null)
            {
                __instance.roleTabs[2].ReceiveClickDown();
                togglePrefab = UnityEngine.Object.Instantiate(__instance.AdvancedRolesSettings.transform.GetChild(9).GetComponent<ToggleOption>(), __instance.transform);
                numberPrefab = UnityEngine.Object.Instantiate(__instance.AdvancedRolesSettings.transform.GetChild(8).GetComponent<NumberOption>(), __instance.transform);
                togglePrefab.gameObject.SetActive(false);
                numberPrefab.gameObject.SetActive(false);
                __instance.roleTabs[0].ReceiveClickDown();
            }
            if (AdvancedTab == null)
            {
                AdvancedTab = UnityEngine.Object.Instantiate(__instance.AdvancedRolesSettings, __instance.AdvancedRolesSettings.transform.parent);
                for (int i = 1; i < AdvancedTab.transform.GetChildCount(); i++)
                {
                    Transform t = AdvancedTab.transform.GetChild(i);
                    if (i == 2)
                    {
                        labelSprite = t.GetChild(0).GetComponent<SpriteRenderer>();
                        labelText = t.GetChild(1).GetComponent<TextMeshPro>();
                    }
                    else
                    {
                        t.gameObject.SetActive(false);
                    }
                }
                AdvancedTab.SetActive(false);
            }
            if (currentPlugin != chanceTabPlugin)
            {
                chanceTabPlugin = currentPlugin;
                for (int i = 10; i < __instance.RoleChancesSettings.transform.GetChildCount(); i++)
                {
                    UnityEngine.Object.Destroy(__instance.RoleChancesSettings.transform.GetChild(i).gameObject);
                }
                if (currentPlugin != FungleAPIPlugin.Plugin)
                {
                    LoadChanceTab(currentPlugin, __instance);
                    for (int i = 9; i >= 0; i--)
                    {
                        __instance.RoleChancesSettings.transform.GetChild(i).gameObject.SetActive(false);
                    }
                }
                else
                {
                    for (int i = 9; i >= 0; i--)
                    {
                        __instance.RoleChancesSettings.transform.GetChild(i).gameObject.SetActive(true);
                    }
                    __instance.AllButton.transform.parent.gameObject.SetActive(true);
                    __instance.scrollBar.transform.localPosition = new Vector3(-1.4957f, 0.657f, -4);
                    __instance.scrollBar.transform.GetChild(1).localScale = new Vector3(6.6811f, 3.5563f, 0.5598f);
                    SetBoundsY(__instance);
                    __instance.scrollBar.ScrollToTop();
                }
            }
            if (__instance.RoleChancesSettings.active)
            {
                if (options.Count > 0)
                {
                    foreach (OptionBehaviour op in options)
                    {
                        UnityEngine.Object.Destroy(op.gameObject);
                    }
                    options.Clear();
                }
                AdvancedTab.SetActive(false);
            }
            SwitchButton.GetComponent<Updater>().Update();
        }
        public static void LoadChanceTab(ModPlugin plugin, RolesSettingsMenu menu)
        {
            Vector3 Pos = new Vector3(4.986f, 0.662f, -2);
            menu.AllButton.transform.parent.gameObject.SetActive(false);
            menu.scrollBar.transform.localPosition = new Vector3(-1.4957f, 1.057f, -4);
            menu.scrollBar.transform.GetChild(1).localScale = new Vector3(6.6811f, 4.2563f, 0.5598f);
            Vector3 vec = menu.scrollBar.Inner.transform.localPosition;
            vec.y = 0.7f;
            menu.scrollBar.Inner.transform.localPosition = vec;
            CategoryHeaderEditRole crewHeader = CreateHeader(menu, new Color(0.4706f, 0.8f, 0.925f), new Color(0.0392f, 0.3412f, 0.451f), new Color(0.0392f, 0.3412f, 0.451f, 0.498f), new Color(0.0549f, 0.3569f, 0.4667f), StringNames.CrewmateRolesHeader.GetString());
            CategoryHeaderEditRole impHeader = CreateHeader(menu, new Color(0.8f, 0.3255f, 0.3255f), new Color(0.5373f, 0.0863f, 0.0863f), new Color(0.5373f, 0.0863f, 0.0863f, 0.498f), new Color(0.3451f, 0.1373f, 0.1373f), StringNames.ImpostorRolesHeader.GetString());
            string[] names = StringNames.CrewmateRolesHeader.GetString().Split(" ");
            CategoryHeaderEditRole neutralHeader = CreateHeader(menu, Utils.Light(Color.gray, 0.7f), Color.gray, Utils.Dark(Color.gray, 0.7f), Utils.Light(Color.gray, 0.9f), names[0] + " " + names[1] + " " + ModdedTeam.Neutrals.TeamName.GetString());
            List<RoleBehaviour> crewmateRoles = new List<RoleBehaviour>();
            List<RoleBehaviour> impostorRoles = new List<RoleBehaviour>();
            List<RoleBehaviour> neutralRoles = new List<RoleBehaviour>();
            List<ModdedTeam> customTeams = new List<ModdedTeam>();
            foreach (RoleBehaviour role in plugin.Roles)
            {
                if (role.GetTeam() == ModdedTeam.Crewmates)
                {
                    crewmateRoles.Add(role);
                }
                else if (role.GetTeam() == ModdedTeam.Impostors)
                {
                    impostorRoles.Add(role);
                }
                else if (role.GetTeam() == ModdedTeam.Neutrals)
                {
                    neutralRoles.Add(role);
                }
                else if (!customTeams.Contains(role.GetTeam()))
                {
                    customTeams.Add(role.GetTeam());
                }
            }
            if (crewmateRoles.Count > 0)
            {
                crewHeader.gameObject.SetActive(true);
                crewHeader.transform.localPosition = Pos;
                Pos = OrganizeRoles(crewmateRoles, Pos, menu);
            }
            if (impostorRoles.Count > 0)
            {
                impHeader.gameObject.SetActive(true);
                impHeader.transform.localPosition = Pos;
                Pos = OrganizeRoles(impostorRoles, Pos, menu);
            }
            if (neutralRoles.Count > 0)
            {
                neutralHeader.gameObject.SetActive(true);
                neutralHeader.transform.localPosition = Pos;
                Pos = OrganizeRoles(neutralRoles, Pos, menu);
            }
            foreach (ModdedTeam team in customTeams)
            {
                List<RoleBehaviour> teamRoles = new List<RoleBehaviour>();
                foreach (RoleBehaviour role in plugin.Roles)
                {
                    if (role.GetTeam() == team)
                    {
                        teamRoles.Add(role);
                    }
                }
                CategoryHeaderEditRole header = CreateHeader(menu, Utils.Light(team.TeamColor, 0.7f), team.TeamColor, Utils.Dark(team.TeamColor, 0.7f), Utils.Light(team.TeamColor, 0.9f), names[0] + " " + names[1] + " " + team.TeamName.GetString());
                header.transform.localPosition = Pos;
                Pos = OrganizeRoles(teamRoles, Pos, menu);
                Transform transform = UnityEngine.Object.Instantiate(menu.RoleChancesSettings.transform.GetChild(2).GetChild(1).gameObject, header.blankLabel.transform).transform;
                transform.localPosition = new Vector3(0f, 0f, -10f);
                int count = team.GetCount();
                TextMeshPro valueText = transform.GetChild(0).GetComponent<TextMeshPro>();
                PassiveButton component = transform.GetChild(1).GetComponent<PassiveButton>();
                PassiveButton component2 = transform.GetChild(2).GetComponent<PassiveButton>();
                valueText.text = count.ToString();
                component.SetNewAction(delegate
                {
                    if (count - 1 >= 0)
                    {
                        int count3 = count;
                        count = count3 - 1;
                        valueText.text = count.ToString();
                        team.SetCount(count);
                    }
                });
                component2.SetNewAction(delegate
                {
                    if (count + 1 <= team.MaxCount)
                    {
                        int count2 = count;
                        count = count2 + 1;
                        valueText.text = count.ToString();
                        team.SetCount(count);
                    }
                });
                header.gameObject.SetActive(true);
            }
            SetBoundsY(menu);
            menu.scrollBar.ScrollToTop();
            menu.AdvancedRolesSettings.transform.GetChild(0).localPosition = new Vector3(1.4041f, -1.3688f, 0);
            menu.AdvancedRolesSettings.transform.GetChild(0).localScale = new Vector3(0.0675f, 0.2094f, 0.5687f);
        }
        public static Vector3 OrganizeRoles(List<RoleBehaviour> roles, Vector3 currentVec, RolesSettingsMenu menu)
        {
            currentVec.y = currentVec.y - 0.522f;
            float x = currentVec.x;
            for (int i = 0; i < roles.Count; i++)
            {
                currentVec.x = menu.RoleChancesSettings.transform.GetChild(2).localPosition.x;
                ICustomRole role = roles[i].CustomRole();
                RoleOptionSetting option = UnityEngine.Object.Instantiate(menu.roleOptionSettingOrigin, menu.RoleChancesSettings.transform);
                option.SetRole(GameOptionsManager.Instance.CurrentGameOptions.RoleOptions, roles[i], 20);
                option.OnValueChanged = new Action<OptionBehaviour>(delegate
                {
                    role.Configuration.localCount.Value = option.RoleMaxCount;
                    role.Configuration.localChance.Value = option.RoleChance;
                    option.UpdateValuesAndText(GameOptionsManager.Instance.CurrentGameOptions.RoleOptions);
                    if (AmongUsClient.Instance.AmHost)
                    {
                        RpcPair pair = CustomRpcManager.CreateRpcPair(PlayerControl.LocalPlayer.NetId);
                        pair.AddRpc(CustomRpcManager.GetInstance<RpcSyncSeetings>(), role);
                        pair.AddRpc(CustomRpcManager.GetInstance<RpcSendNotification>(), (TranslationController.Instance.GetString(StringNames.LobbyChangeSettingNotificationRole).Replace("{0}", role.RoleColor.ToTextColor() + role.RoleName.GetString() + "</color>").Replace("{1}", role.RoleCount.ToString()).Replace("{2}", role.RoleChance.ToString()), false, true));
                        pair.SendPair();
                    }
                });
                option.UpdateValuesAndText(GameOptionsManager.Instance.CurrentGameOptions.RoleOptions);
                option.SetClickMask(menu.ButtonClickMask);
                option.transform.localPosition = currentVec;
                option.titleText.text = role.RoleName.GetString();
                option.labelSprite.color = role.RoleColor;
                int count = role.RoleCount;
                int chance = role.RoleChance;
                option.countText.text = count.ToString();
                option.chanceText.text = chance.ToString();
                GameOptionButton cog = UnityEngine.Object.Instantiate(option.ChanceMinusBtn, option.transform);
                cog.transform.GetChild(0).gameObject.SetActive(false);
                cog.buttonSprite.sprite = Cog;
                cog.transform.localPosition = new Vector3(-1.278f, -0.3f, 0f);
                cog.transform.localScale = new Vector3(1, 1, 1);
                cog.SetNewAction(delegate
                {
                    if (role.Configuration.Configs.Count() > 0)
                    {
                        GameSettingMenu.Instance.MenuDescriptionText.text = role.RoleBlurLong.GetString();
                        menu.AdvancedRolesSettings.gameObject.SetActive(false);
                        menu.RoleChancesSettings.gameObject.SetActive(false);
                        AdvancedTab.SetActive(true);
                        labelSprite.color = Utils.Light(role.RoleColor);
                        labelText.text = role.RoleName.GetString();
                        labelText.color = role.RoleColor;
                        for (int i = 0; i < role.Configuration.Configs.Count(); i++)
                        {
                            CustomOption config = role.Configuration.Configs[i];
                            OptionBehaviour op = config.CreateOption(AdvancedTab.transform);
                            op.OnValueChanged = new Action<OptionBehaviour>(delegate
                            {
                                RpcPair pair = CustomRpcManager.CreateRpcPair(PlayerControl.LocalPlayer.NetId);
                                pair.AddRpc(CustomRpcManager.GetInstance<RpcSyncSeetings>(), role);
                                pair.AddRpc(CustomRpcManager.GetInstance<RpcSendNotification>(), (TranslationController.Instance.GetString(StringNames.LobbyChangeSettingNotification).Replace("{0}", role.RoleColor.ToTextColor() + "(" + role.RoleName.GetString() + ") " + config.ConfigName + "</color>").Replace("{1}", config.GetValue()), false, true));
                                pair.SendPair();
                            });
                            op.transform.localPosition = new Vector3(0.5f, 0.1f - 0.4f * i, -10f);
                            options.Add(op);
                        }
                    }
                });
                cog.gameObject.AddComponent<Updater>().onUpdate = new Action(delegate
                {
                    cog.SetInteractable(role.Configuration.Configs.Count() > 0);
                });
                currentVec.y = currentVec.y - 0.43f;
                option.gameObject.SetActive(true);
            }
            currentVec.y = currentVec.y - 0.65f;
            currentVec.x = x;
            return currentVec;
        }
        public static CategoryHeaderEditRole CreateHeader(RolesSettingsMenu menu, Color background, Color label1, Color label2, Color titleColor, string titleText)
        {

            CategoryHeaderEditRole header = UnityEngine.Object.Instantiate(menu.categoryHeaderEditRoleOrigin, menu.RoleChancesSettings.transform);
            header.SetHeader(StringNames.None, 20);
            header.Background.color = background;
            header.countLabel.color = label1;
            header.chanceLabel.color = label1;
            header.blankLabel.color = label2;
            header.Title.text = titleText;
            header.Title.color = titleColor;
            header.gameObject.SetActive(false);
            return header;
        }
        public static Sprite Cog = ResourceHelper.LoadSprite(FungleAPIPlugin.Plugin, "FungleAPI.Resources.cog", 200f);
    }
}
