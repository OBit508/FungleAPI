using AmongUs.GameOptions;
using HarmonyLib;
using System.Collections.Generic;
using UnityEngine;
using FungleAPI.PluginLoading;
using FungleAPI.Teams;
using FungleAPI.Role.Utilities;
using FungleAPI.GameOptions.Lobby;
using FungleAPI.Api;

namespace FungleAPI.GameOptions.Patches
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
                __instance.scrollBar.CalculateAndSetYBounds(__instance.RoleChancesSettings.transform.GetChildCount() + (GameSettingMenuPatch.pluginChanger.CurrentPlugin == FungleApiPlugin.Plugin ? 3 : 0), 1f, 6f, 0.43f);
                __instance.scrollBar.ScrollToTop();
            }
            Transform mask = __instance.scrollBar.transform.GetChild(1);
            Vector3 localPosition = mask.localPosition;
            localPosition.y = GameSettingMenuPatch.pluginChanger.CurrentPlugin == FungleApiPlugin.Plugin ? -0.5734f : -0.1734f;
            Vector3 localScale = mask.localScale;
            localScale.y = GameSettingMenuPatch.pluginChanger.CurrentPlugin == FungleApiPlugin.Plugin ? 3.5563f : 4.3563f;
            mask.localPosition = localPosition;
            mask.localScale = localScale;
            __instance.AllButton.transform.parent.parent.gameObject.SetActive(GameSettingMenuPatch.pluginChanger.CurrentPlugin == FungleApiPlugin.Plugin);
            __instance.RoleChancesSettings.transform.localPosition = new Vector3(0, GameSettingMenuPatch.pluginChanger.CurrentPlugin == FungleApiPlugin.Plugin ? 0 : 0.9f, -5);
            __instance.AdvancedRolesSettings.transform.localPosition = __instance.RoleChancesSettings.transform.localPosition;
        }
        [HarmonyPatch("Awake")]
        [HarmonyPostfix]
        public static void AwakePostfix(RolesSettingsMenu __instance)
        {
            chanceTabPlugin = FungleApiPlugin.Plugin;
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
            if (GameSettingMenuPatch.pluginChanger.CurrentPlugin != FungleApiPlugin.Plugin && GameSettingMenuPatch.CurrentTab is RoleTab roleTab)
            {
                roleTab.SetQuotaTab(__instance, Headers);
            }
            else
            {
                float num = 0.662f;
                float num2 = -1.928f;
                __instance.roleTabs = new Il2CppSystem.Collections.Generic.List<PassiveButton>();
                __instance.roleTabs.Add(__instance.AllButton);
                List<RoleBehaviour> list = FungleApiPlugin.Plugin.Roles.FindAll(r => r.GetTeam() == ModdedTeamManager.Crewmates && r.Role != RoleTypes.Crewmate && r.Role != RoleTypes.CrewmateGhost);
                List<RoleBehaviour> list2 = FungleApiPlugin.Plugin.Roles.FindAll(r => r.GetTeam() == ModdedTeamManager.Impostors && r.Role != RoleTypes.Impostor && r.Role != RoleTypes.ImpostorGhost);
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
    }
}
