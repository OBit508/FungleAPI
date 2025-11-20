using AmongUs.GameOptions;
using BepInEx.Unity.IL2CPP.Utils.Collections;
using Discord;
using Epic.OnlineServices;
using FungleAPI.Utilities.Assets;
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
using FungleAPI.Configuration.Attributes;
using FungleAPI.PluginLoading;

namespace FungleAPI.Configuration.Patches
{
    [HarmonyPatch(typeof(RolesSettingsMenu))]
    internal static class RolesSettingMenuPatch
    {
        public static ModPlugin chanceTabPlugin;
        public static List<CategoryHeaderEditRole> Headers = new List<CategoryHeaderEditRole>();
        [HarmonyPatch("Update")]
        [HarmonyPrefix]
        public static void UpdatePrefix(RolesSettingsMenu __instance)
        {
            if (GameSettingMenuPatch.pluginChanger.CurrentPlugin != chanceTabPlugin)
            {
                __instance.SetQuotaTab();
                chanceTabPlugin = GameSettingMenuPatch.pluginChanger.CurrentPlugin;
                __instance.scrollBar.CalculateAndSetYBounds(__instance.RoleChancesSettings.transform.GetChildCount() + (GameSettingMenuPatch.pluginChanger.CurrentPlugin == FungleAPIPlugin.Plugin ? 3 : 0), 1f, 6f, 0.43f);
                __instance.scrollBar.ScrollToTop();
            }
            Transform mask = __instance.scrollBar.transform.GetChild(1);
            Vector3 localPosition = mask.localPosition;
            localPosition.y = GameSettingMenuPatch.pluginChanger.CurrentPlugin == FungleAPIPlugin.Plugin ? -0.5734f : -0.1734f;
            Vector3 localScale = mask.localScale;
            localScale.y = GameSettingMenuPatch.pluginChanger.CurrentPlugin == FungleAPIPlugin.Plugin ? 3.5563f : 4.3563f;
            mask.localPosition = localPosition;
            mask.localScale = localScale;
            __instance.AllButton.transform.parent.parent.gameObject.SetActive(GameSettingMenuPatch.pluginChanger.CurrentPlugin == FungleAPIPlugin.Plugin);
            __instance.RoleChancesSettings.transform.localPosition = new Vector3(0, GameSettingMenuPatch.pluginChanger.CurrentPlugin == FungleAPIPlugin.Plugin ? 0 : 0.9f, -5);
            __instance.AdvancedRolesSettings.transform.localPosition = __instance.RoleChancesSettings.transform.localPosition;
        }
        [HarmonyPatch("Awake")]
        [HarmonyPostfix]
        public static void AwakePostfix(RolesSettingsMenu __instance)
        {
            chanceTabPlugin = FungleAPIPlugin.Plugin;
            Headers.Clear();
        }
        [HarmonyPatch("SetQuotaTab")]
        [HarmonyPrefix]
        public static bool SetQuotaTabPrefix(RolesSettingsMenu __instance)
        {
            for (int i = 0; i < __instance.RoleChancesSettings.transform.GetChildCount(); i++)
            {
                if (__instance.RoleChancesSettings.transform.GetChild(i) != __instance.quotaHeader.transform)
                {
                    UnityEngine.Object.Destroy(__instance.RoleChancesSettings.transform.GetChild(i).gameObject);
                }
            }
            if (__instance.roleTabs != null)
            {
                foreach (PassiveButton button in __instance.roleTabs)
                {
                    if (button != __instance.AllButton)
                    {
                        UnityEngine.Object.Destroy(button.gameObject);
                    }
                }
            }
            if (GameSettingMenuPatch.pluginChanger.CurrentPlugin != FungleAPIPlugin.Plugin)
            {
                SetQuotaTab(__instance);
            }
            else
            {
                float num = 0.662f;
                float num2 = -1.928f;
                __instance.roleTabs = new Il2CppSystem.Collections.Generic.List<PassiveButton>();
                __instance.roleTabs.Add(__instance.AllButton);
                List<RoleBehaviour> list = FungleAPIPlugin.Plugin.Roles.FindAll(r => r.GetTeam() == ModdedTeam.Crewmates && r.Role != RoleTypes.Crewmate && r.Role != RoleTypes.CrewmateGhost);
                List<RoleBehaviour> list2 = FungleAPIPlugin.Plugin.Roles.FindAll(r => r.GetTeam() == ModdedTeam.Impostors && r.Role != RoleTypes.Impostor && r.Role != RoleTypes.ImpostorGhost);
                for (int i = 0; i < list.Count; i++)
                {
                    __instance.AddRoleTab(list[i], ref num2);
                }
                for (int j = 0; j < list2.Count; j++)
                {
                    __instance.AddRoleTab(list2[j], ref num2);
                }
                CategoryHeaderEditRole categoryHeaderEditRole = UnityEngine.Object.Instantiate(__instance.categoryHeaderEditRoleOrigin, Vector3.zero, Quaternion.identity, __instance.RoleChancesSettings.transform);
                categoryHeaderEditRole.SetHeader(StringNames.CrewmateRolesHeader, 20);
                categoryHeaderEditRole.transform.localPosition = new Vector3(4.986f, num, -2f);
                Headers.Add(categoryHeaderEditRole);
                num -= 0.522f;
                int num3 = 0;
                for (int k = 0; k < list.Count; k++)
                {
                    __instance.CreateQuotaOption(list[k], ref num, num3);
                    num3++;
                }
                num -= 0.22f;
                CategoryHeaderEditRole categoryHeaderEditRole2 = UnityEngine.Object.Instantiate(__instance.categoryHeaderEditRoleOrigin, Vector3.zero, Quaternion.identity, __instance.RoleChancesSettings.transform);
                categoryHeaderEditRole2.SetHeader(StringNames.ImpostorRolesHeader, 20);
                categoryHeaderEditRole2.transform.localPosition = new Vector3(4.986f, num, -2f);
                Headers.Add(categoryHeaderEditRole2);
                num -= 0.522f;
                for (int l = 0; l < list2.Count; l++)
                {
                    __instance.CreateQuotaOption(list2[l], ref num, num3);
                    num3++;
                }
            }
            return false;
        }
        public static void SetQuotaTab(RolesSettingsMenu menu)
        {
            float num = 0.662f;
            menu.roleTabs = new Il2CppSystem.Collections.Generic.List<PassiveButton>();
            menu.roleTabs.Add(menu.AllButton);
            Dictionary<ModdedTeam, List<RoleBehaviour>> teams = GameSettingMenuPatch.pluginChanger.CurrentPlugin.GetTeamsAndRoles();
            foreach (KeyValuePair<ModdedTeam, List<RoleBehaviour>> pair in teams)
            {
                List<RoleBehaviour> validRoles = pair.Value;
                validRoles.RemoveAll(r => r.CustomRole() != null && r.CustomRole().HideRole);
                if (validRoles.Count > 0)
                {
                    CategoryHeaderEditRole categoryHeaderEditRole = pair.Key.CreatCategoryHeaderEditRole(menu.RoleChancesSettings.transform);
                    categoryHeaderEditRole.transform.localPosition = new Vector3(4.986f, num, -2f);
                    Headers.Add(categoryHeaderEditRole);
                    num -= 0.522f;
                    foreach (RoleBehaviour role in validRoles)
                    {
                        if (role.CustomRole() != null)
                        {
                            menu.CreateQuotaOption(role.CustomRole(), ref num);
                        }
                    }
                    num -= 0.22f;
                }
            }
        }
        public static void CreateQuotaOption(this RolesSettingsMenu menu, ICustomRole role, ref float yPos)
        {
            RoleOptionSetting option = UnityEngine.Object.Instantiate(menu.roleOptionSettingOrigin, menu.RoleChancesSettings.transform);
            option.SetRole(GameOptionsManager.Instance.CurrentGameOptions.RoleOptions, role as RoleBehaviour, 20);
            option.OnValueChanged = new Action<OptionBehaviour>(delegate
            {
                role.CountAndChance.SetCount(option.RoleMaxCount);
                role.CountAndChance.SetChance(option.RoleChance);
                option.UpdateValuesAndText(GameOptionsManager.Instance.CurrentGameOptions.RoleOptions);
                if (AmongUsClient.Instance.AmHost)
                {
                    CustomRpcManager.Instance<RpcSyncRoleCountAndChance>().Send(role as RoleBehaviour, PlayerControl.LocalPlayer.NetId);
                }
            });
            option.roleMaxCount = role.MaxRoleCount;
            option.UpdateValuesAndText(GameOptionsManager.Instance.CurrentGameOptions.RoleOptions);
            option.SetClickMask(menu.ButtonClickMask);
            option.transform.localPosition = new Vector3(-0.15f, yPos, -2f);
            option.titleText.text = role.RoleName.GetString();
            option.labelSprite.color = role.RoleColor;
            int count = role.CountAndChance.GetCount();
            int chance = role.CountAndChance.GetChance();
            option.countText.text = count.ToString();
            option.chanceText.text = chance.ToString();
            if (role.Options.Count > 0)
            {
                PassiveButton cog = FungleAssets.CogPrefab.Instantiate(option.transform).GetComponent<PassiveButton>();
                cog.transform.localPosition = new Vector3(-1.278f, -0.3f, 0f);
                cog.ClickMask = menu.ButtonClickMask;
                cog.SetNewAction(delegate
                {
                    ChangeTab(menu, role);
                });
            }
            option.gameObject.SetActive(true);
            menu.roleChances.Add(option);
            yPos += -0.43f;
        }
        public static void ChangeTab(RolesSettingsMenu menu, ICustomRole role)
        {
            menu.roleDescriptionText.transform.parent.localPosition = new Vector3(2.5176f, -0.2731f, -1f);
            menu.roleDescriptionText.transform.parent.localScale = new Vector3(0.0675f, 0.1494f, 0.5687f);
            menu.AdvancedRolesSettings.transform.FindChild("InfoLabelBackground").transform.localPosition = new Vector3(1.082f, 0.1054f, -2.5f);
            menu.roleScreenshot.sprite = null;
            if (role.Screenshot != null)
            {
                menu.roleScreenshot.sprite = Sprite.Create(role.Screenshot.texture, new UnityEngine.Rect(0f, 0f, 370f, 230f), Vector2.one / 2f, 100f);
            }
            Transform transform = menu.AdvancedRolesSettings.transform.Find("Background");
            transform.localPosition = new Vector3(1.4041f, -7.08f, 0f);
            transform.GetComponent<SpriteRenderer>().size = new Vector2(89.4628f, 100f);
            for (int i = 0; i < menu.advancedSettingChildren.Count; i++)
            {
                UnityEngine.Object.Destroy(menu.advancedSettingChildren[i].gameObject);
            }
            menu.ControllerSelectable.Clear();
            menu.advancedSettingChildren.Clear();
            menu.roleDescriptionText.text = role.RoleBlurLong.GetString();
            menu.roleTitleText.text = role.RoleName.GetString();
            float num = -0.872f;
            menu.roleHeaderSprite.color = Utilities.Helpers.Light(role.RoleColor);
            menu.roleHeaderText.color = role.RoleColor;
            menu.roleHeaderText.text = role.RoleName.GetString();
            foreach (ModdedOption config in role.Options)
            {
                OptionBehaviour op = config.CreateOption(menu.AdvancedRolesSettings.transform);
                op.SetClickMask(menu.ButtonClickMask);
                op.OnValueChanged += new Action<OptionBehaviour>(delegate
                {
                    CustomRpcManager.Instance<RpcSyncSettings>().Send((SyncTextType.RoleOption, config, role as RoleBehaviour, null), PlayerControl.LocalPlayer.NetId);
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
    }
}
