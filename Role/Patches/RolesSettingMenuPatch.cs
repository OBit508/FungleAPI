using AmongUs.GameOptions;
using BepInEx.Unity.IL2CPP.Utils.Collections;
using Discord;
using Epic.OnlineServices;
using FungleAPI.Utilities.Assets;
using FungleAPI.Configuration;
using FungleAPI.Components;
using FungleAPI.Networking.RPCs;
using FungleAPI.Role.Teams;
using FungleAPI.Role;
using FungleAPI.Networking;
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
using static Rewired.Controller;
using static Rewired.Platforms.Custom.CustomPlatformUnifiedKeyboardSource.KeyPropertyMap;
using static Rewired.UI.ControlMapper.ControlMapper;

namespace FungleAPI.Role.Patches
{
    [HarmonyPatch(typeof(RolesSettingsMenu))]
    internal static class RolesSettingMenuPatch
    {
        public static ModPlugin currentPlugin;
        public static ModPlugin chanceTabPlugin;
        public static PassiveButton SwitchButton;
        public static int currentIndex;
        public static Dictionary<ModPlugin, Transform> Pages = new Dictionary<ModPlugin, Transform>();
        public static void Reset()
        {
            currentPlugin = FungleAPIPlugin.Plugin;
            chanceTabPlugin = null;
        }
        public static void SetBoundsY(RolesSettingsMenu menu, Transform target)
        {
            menu.scrollBar.ContentYBounds.min = currentPlugin == FungleAPIPlugin.Plugin ? 0 : 0.7f;
            menu.scrollBar.CalculateAndSetYBounds(target.GetChildCount() + (currentPlugin == FungleAPIPlugin.Plugin ? 3 : 0), 1f, 6f, 0.43f);
        }
        [HarmonyPatch("Awake")]
        [HarmonyPostfix]
        public static void AwakePostfix(RolesSettingsMenu __instance)
        {
            Pages.Clear();
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
            SwitchButton.gameObject.AddComponent<Updater>().update = new Action(delegate
            {
                __instance.transform.parent.parent.GetChild(2).gameObject.SetActive(!__instance.isActiveAndEnabled && !__instance.AdvancedRolesSettings.active);
                SwitchButton.gameObject.SetActive(__instance.isActiveAndEnabled && !__instance.AdvancedRolesSettings.active);
                SwitchButton.transform.GetChild(0).GetChild(0).GetComponent<TextMeshPro>().text = currentPlugin.ModName;
            });
            Pages.Add(FungleAPIPlugin.Plugin, __instance.RoleChancesSettings.transform);
            foreach (ModPlugin plugin in ModPlugin.AllPlugins)
            {
                if (plugin != FungleAPIPlugin.Plugin)
                {
                    Transform modParent = new GameObject(plugin.ModName + "Tab").transform;
                    modParent.SetParent(__instance.RoleChancesSettings.transform.parent);
                    modParent.transform.localPosition = __instance.RoleChancesSettings.transform.localPosition;
                    CreateChanceTab(plugin, __instance, modParent);
                    Pages.Add(plugin, modParent);
                }
            }
        }
        [HarmonyPatch("OpenChancesTab")]
        [HarmonyPostfix]
        public static void OpenChancesTabPostfix(RolesSettingsMenu __instance, [HarmonyArgument(0)] bool controllerSelected)
        {
            __instance.AdvancedRolesSettings.SetActive(false);
            foreach (KeyValuePair<ModPlugin, Transform> pair in Pages)
            {
                pair.Value.gameObject.SetActive(currentPlugin == pair.Key && !__instance.AdvancedRolesSettings.active);
            }
        }
        [HarmonyPatch("SetQuotaTab")]
        [HarmonyPrefix]
        public static bool SetQuotaTabPrefix(RolesSettingsMenu __instance)
        {
            float num = 0.662f;
            float num2 = -1.928f;
            __instance.roleTabs = new Il2CppSystem.Collections.Generic.List<PassiveButton>();
            __instance.roleTabs.Add(__instance.AllButton);
            List<RoleBehaviour> list = DestroyableSingleton<RoleManager>.Instance.AllRoles.ToSystemList().FindAll((RoleBehaviour r) => r.TeamType == RoleTeamTypes.Crewmate && r.Role != RoleTypes.Crewmate && r.Role != RoleTypes.CrewmateGhost && r.CustomRole() == null);
            List<RoleBehaviour> list2 = DestroyableSingleton<RoleManager>.Instance.AllRoles.ToSystemList().FindAll((RoleBehaviour r) => r.TeamType == RoleTeamTypes.Impostor && r.Role != RoleTypes.Impostor && r.Role != RoleTypes.ImpostorGhost && r.CustomRole() == null);
            for (int i = 0; i < list.Count; i++)
            {
                __instance.AddRoleTab(list[i], ref num2);
            }
            for (int j = 0; j < list2.Count; j++)
            {
                __instance.AddRoleTab(list2[j], ref num2);
            }
            CategoryHeaderEditRole categoryHeaderEditRole = GameObject.Instantiate<CategoryHeaderEditRole>(__instance.categoryHeaderEditRoleOrigin, Vector3.zero, Quaternion.identity, __instance.RoleChancesSettings.transform);
            categoryHeaderEditRole.SetHeader(StringNames.CrewmateRolesHeader, 20);
            categoryHeaderEditRole.transform.localPosition = new Vector3(4.986f, num, -2f);
            num -= 0.522f;
            int num3 = 0;
            for (int k = 0; k < list.Count; k++)
            {
                __instance.CreateQuotaOption(list[k], ref num, num3);
                num3++;
            }
            num -= 0.22f;
            CategoryHeaderEditRole categoryHeaderEditRole2 = GameObject.Instantiate<CategoryHeaderEditRole>(__instance.categoryHeaderEditRoleOrigin, Vector3.zero, Quaternion.identity, __instance.RoleChancesSettings.transform);
            categoryHeaderEditRole2.SetHeader(StringNames.ImpostorRolesHeader, 20);
            categoryHeaderEditRole2.transform.localPosition = new Vector3(4.986f, num, -2f);
            num -= 0.522f;
            for (int l = 0; l < list2.Count; l++)
            {
                __instance.CreateQuotaOption(list2[l], ref num, num3);
                num3++;
            }
            return false;
        }
        [HarmonyPatch("Update")]
        [HarmonyPrefix]
        public static void UpdatePrefix(RolesSettingsMenu __instance)
        {
            __instance.AllButton.transform.parent.parent.gameObject.SetActive(chanceTabPlugin == FungleAPIPlugin.Plugin);
            foreach (KeyValuePair<ModPlugin, Transform> pair in Pages)
            {
                pair.Value.gameObject.SetActive(currentPlugin == pair.Key && !__instance.AdvancedRolesSettings.active);
                pair.Value.localScale = __instance.RoleChancesSettings.transform.localScale;
            }
            if (currentPlugin != chanceTabPlugin)
            {
                chanceTabPlugin = currentPlugin;
                SetBoundsY(__instance, Pages[chanceTabPlugin]);
                __instance.scrollBar.ScrollToTop();
            }
            Vector3 localPosition = __instance.scrollBar.transform.GetChild(1).localPosition;
            localPosition.y = currentPlugin != FungleAPIPlugin.Plugin ? 0.2266f : -0.5734f;
            __instance.scrollBar.transform.GetChild(1).localPosition = localPosition;
            SwitchButton.GetComponent<Updater>().Update();
        }
        public static void ChangeTab(RolesSettingsMenu menu, ICustomRole role)
        {
            menu.roleDescriptionText.transform.parent.localPosition = new Vector3(2.5176f, -0.2731f, -1f);
            menu.roleDescriptionText.transform.parent.localScale = new Vector3(0.0675f, 0.1494f, 0.5687f);
            menu.AdvancedRolesSettings.transform.FindChild("InfoLabelBackground").transform.localPosition = new Vector3(1.082f, 0.1054f, -2.5f);
            if (role.Configuration.Screenshot != null)
            {
                menu.roleScreenshot.sprite = Sprite.Create(role.Configuration.Screenshot.texture, new UnityEngine.Rect(0f, 0f, 370f, 230f), Vector2.one / 2f, 100f);
            }
            menu.roleScreenshot.drawMode = SpriteDrawMode.Sliced;
            Transform transform = menu.AdvancedRolesSettings.transform.Find("Background");
            transform.localPosition = new Vector3(1.4041f, -7.08f, 0f);
            transform.GetComponent<SpriteRenderer>().size = new Vector2(89.4628f, 100f);
            for (int i = 0; i < menu.advancedSettingChildren.Count; i++)
            {
                GameObject.Destroy(menu.advancedSettingChildren[i].gameObject);
            }
            menu.ControllerSelectable.Clear();
            menu.advancedSettingChildren.Clear();
            menu.roleDescriptionText.text = role.RoleBlurLong.GetString();
            menu.roleTitleText.text = role.RoleName.GetString();
            menu.roleScreenshot.sprite = role.Configuration.Screenshot;
            float num = -0.872f;
            menu.roleHeaderSprite.color = Helpers.Light(role.RoleColor);
            menu.roleHeaderText.color = role.RoleColor;
            menu.roleHeaderText.text = role.RoleName.GetString();
            foreach (ModdedOption config in role.Configuration.Configs)
            {
                OptionBehaviour op = config.CreateOption(menu.AdvancedRolesSettings.transform);
                op.SetClickMask(menu.ButtonClickMask);
                op.OnValueChanged += new Action<OptionBehaviour>(delegate
                {
                    CustomRpcManager.Instance<RpcSyncSeetings>().Send((role, TranslationController.Instance.GetString(StringNames.LobbyChangeSettingNotification).Replace("{0}", role.RoleColor.ToTextColor() + "(" + role.RoleName.GetString() + ") " + config.ConfigName + "</color>").Replace("{1}", config.GetValue()), false, true), PlayerControl.LocalPlayer.NetId);
                });
                op.transform.localPosition = new Vector3(2.17f, num, -2f);
                menu.advancedSettingChildren.Add(op);
                num += -0.45f;
            }
            menu.scrollBar.CalculateAndSetYBounds(menu.AdvancedRolesSettings.transform.GetChildCount(), 1f, 6f, 0.45f);
            menu.scrollBar.ScrollToTop();
            menu.RoleChancesSettings.SetActive(false);
            menu.AdvancedRolesSettings.SetActive(true);
            menu.RefreshChildren();
            menu.InitializeControllerNavigation();
        }
        public static void CreateChanceTab(ModPlugin plugin, RolesSettingsMenu menu, Transform parent)
        {
            Vector3 Pos = new Vector3(4.986f, 1.162f, -2);
            CategoryHeaderEditRole crewHeader = CreateHeader(menu, new Color(0.4706f, 0.8f, 0.925f), new Color(0.0392f, 0.3412f, 0.451f), new Color(0.0392f, 0.3412f, 0.451f, 0.498f), new Color(0.0549f, 0.3569f, 0.4667f), StringNames.CrewmateRolesHeader.GetString(), parent);
            CategoryHeaderEditRole impHeader = CreateHeader(menu, new Color(0.8f, 0.3255f, 0.3255f), new Color(0.5373f, 0.0863f, 0.0863f), new Color(0.5373f, 0.0863f, 0.0863f, 0.498f), new Color(0.3451f, 0.1373f, 0.1373f), StringNames.ImpostorRolesHeader.GetString(), parent);
            string[] names = StringNames.CrewmateRolesHeader.GetString().Split(" ");
            CategoryHeaderEditRole neutralHeader = CreateHeader(menu, Helpers.Light(Color.gray, 0.7f), Color.gray, Helpers.Dark(Color.gray, 0.7f), Helpers.Light(Color.gray, 0.9f), names[0] + " " + names[1] + " " + ModdedTeam.Neutrals.TeamName.GetString(), parent);
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
                Pos = OrganizeRoles(crewmateRoles, Pos, menu, parent);
            }
            if (impostorRoles.Count > 0)
            {
                impHeader.gameObject.SetActive(true);
                impHeader.transform.localPosition = Pos;
                Pos = OrganizeRoles(impostorRoles, Pos, menu, parent);
            }
            if (neutralRoles.Count > 0)
            {
                neutralHeader.gameObject.SetActive(true);
                neutralHeader.transform.localPosition = Pos;
                Pos = OrganizeRoles(neutralRoles, Pos, menu, parent);
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
                CategoryHeaderEditRole header = CreateHeader(menu, Helpers.Light(team.TeamColor, 0.7f), team.TeamColor, Helpers.Dark(team.TeamColor, 0.7f), Helpers.Light(team.TeamColor, 0.9f), names[0] + " " + names[1] + " " + team.TeamName.GetString(), parent);
                header.transform.localPosition = Pos;
                Pos = OrganizeRoles(teamRoles, Pos, menu, parent);
                header.gameObject.SetActive(true);
            }
            Vector3 vec = menu.scrollBar.transform.GetChild(1).localPosition;
            vec.y = 0.2266f;
        }
        public static Vector3 OrganizeRoles(List<RoleBehaviour> roles, Vector3 currentVec, RolesSettingsMenu menu, Transform parent)
        {
            currentVec.y = currentVec.y - 0.522f;
            float x = currentVec.x;
            for (int i = 0; i < roles.Count; i++)
            {
                currentVec.x = -0.15f;
                ICustomRole role = roles[i].CustomRole();
                RoleOptionSetting option = UnityEngine.Object.Instantiate(menu.roleOptionSettingOrigin, parent);
                option.SetRole(GameOptionsManager.Instance.CurrentGameOptions.RoleOptions, roles[i], 20);
                option.OnValueChanged = new Action<OptionBehaviour>(delegate
                {
                    role.Configuration.CountAndChance.SetCount(option.RoleMaxCount);
                    role.Configuration.CountAndChance.SetChance(option.RoleChance);
                    option.UpdateValuesAndText(GameOptionsManager.Instance.CurrentGameOptions.RoleOptions);
                    if (AmongUsClient.Instance.AmHost)
                    {
                        CustomRpcManager.Instance<RpcSyncSeetings>().Send((role, TranslationController.Instance.GetString(StringNames.LobbyChangeSettingNotificationRole).Replace("{0}", role.RoleColor.ToTextColor() + role.RoleName.GetString() + "</color>").Replace("{1}", role.RoleCount.ToString()).Replace("{2}", role.RoleChance.ToString()), false, true), PlayerControl.LocalPlayer.NetId);
                    }
                });
                option.roleMaxCount = role.Configuration.MaxRoleCount;
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
                    ChangeTab(menu, role);
                });
                cog.gameObject.AddComponent<Updater>().update = new Action(delegate
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
        public static CategoryHeaderEditRole CreateHeader(RolesSettingsMenu menu, Color background, Color label1, Color label2, Color titleColor, string titleText, Transform parent)
        {

            CategoryHeaderEditRole header = UnityEngine.Object.Instantiate(menu.categoryHeaderEditRoleOrigin, parent);
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
        public static Sprite Cog;
    }
}
