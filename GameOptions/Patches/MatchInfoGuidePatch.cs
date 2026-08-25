using AmongUs.GameOptions;
using FungleAPI.Api;
using FungleAPI.Components;
using FungleAPI.GameModes;
using FungleAPI.PluginLoading;
using FungleAPI.Role.Utilities;
using HarmonyLib;
using Steamworks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;

namespace FungleAPI.GameOptions.Patches
{
    [HarmonyPatch(typeof(MatchInfoGuide))]
    internal static class MatchInfoGuidePatch
    {
        public static PluginChanger pluginChanger;

        public static Dictionary<Assembly, Transform> RolesParents = new Dictionary<Assembly, Transform>();

        [HarmonyPatch(nameof(MatchInfoGuide.Awake))]
        [HarmonyPostfix]
        public static void AwakePostfix(MatchInfoGuide __instance)
        {
            RolesParents.Clear();

            Transform parent = __instance.transform.GetChild(0);

            parent.GetChild(5).GetComponent<TextMeshPro>().alignment = TextAlignmentOptions.Right;

            pluginChanger = GameObject.Instantiate(FungleAssets.PluginChangerPrefab, parent);
            pluginChanger.transform.localPosition = new Vector3(-1.45f, 1.4f, -1);
            pluginChanger.transform.localScale = Vector3.one * 0.3f;

            pluginChanger.Plugins = OptionManager.GetAllAssembliesWithTabs();

            pluginChanger.OnChange = delegate (Assembly assembly)
            {
                foreach (Transform transform in RolesParents.Values)
                {
                    transform.gameObject.SetActive(false);
                }
                if (RolesParents.TryGetValue(assembly, out Transform tr))
                {
                    tr.gameObject.SetActive(true);
                    __instance.rolesEnabledMessage.SetActive(tr.GetChildCount() == 0);
                    if (!__instance.rolesEnabledMessage.activeSelf)
                    {
                        __instance.MatchInfoRoleScroller.SetYBoundsMax(Mathf.Clamp(Mathf.Ceil((float)tr.GetChildCount() / 2f) + __instance.RoleEntryBoundsModifier, 0f, 999f));
                    }
                }
            };
        }
        [HarmonyPatch(nameof(MatchInfoGuide.CreateNormalModeSettings))]
        [HarmonyPrefix]
        public static bool CreateNormalModeSettingsPrefix(MatchInfoGuide __instance)
        {
            BaseGameMode baseGameMode = GameModeManager.GetCurrentGameMode();
            if (baseGameMode is NormalGameMode)
            {
                __instance.CreateSettingsEntry(StringNames.GameNumImpostors, Enumerable.Count<NetworkedPlayerInfo>(GameData.Instance.AllPlayers.ToArray(), (NetworkedPlayerInfo p) => p.Role.IsImpostor).ToString());
                __instance.CreateSettingsEntry(StringNames.GameKillCooldown, GameManager.Instance.AllGameSettingData[StringNames.GameKillCooldown].GetValueString(GameManager.Instance.LogicOptions.GetKillCooldown()));
                __instance.CreateSettingsEntry(StringNames.GameEmergencyCooldown, GameManager.Instance.AllGameSettingData[StringNames.GameEmergencyCooldown].GetValueString((float)GameManager.Instance.LogicOptions.GetEmergencyCooldown()));
                __instance.CreateSettingsEntry(StringNames.GameVisualTasks, __instance.GetBoolString(GameManager.Instance.LogicOptions.GetVisualTasks()));
                __instance.CreateSettingsEntry(StringNames.GameAnonymousVotes, __instance.GetBoolString(GameManager.Instance.LogicOptions.GetAnonymousVotes()));
                __instance.CreateSettingsEntry(StringNames.GameConfirmImpostor, __instance.GetBoolString(GameManager.Instance.LogicOptions.GetConfirmImpostor()));
                __instance.CreateSettingsEntry(StringNames.GameTaskBarMode, GameManager.Instance.LogicOptions.GetTaskBarMode().ToString());
            }
            else if (baseGameMode.ModeOptions != null)
            {
                foreach (IModdedOption moddedOption in baseGameMode.ModeOptions.OptionCollection.Options)
                {
                    __instance.CreateSettingsEntry(moddedOption.Data.Title, moddedOption.GetStringValue(AmongUsClient.Instance.AmHost));
                }

                __instance.MatchInfoParent.transform.GetChild(7).GetChild(0).GetComponent<Scroller>().ContentYBounds.max = baseGameMode.ModeOptions.OptionCollection.Options.Count / 2 * 0.2f;
            }

            foreach (Assembly assembly in pluginChanger.Plugins)
            {
                RolesParents[assembly] = new GameObject(assembly.GetName().Name)
                {
                    transform =
                    {
                        parent = __instance.MatchInfoRoleScroller.Inner,
                        localPosition = Vector3.zero,
                        localScale = Vector3.one
                    }
                }.transform;
            }

            foreach (ModPlugin modPlugin in ModPluginManager.AllPlugins)
            {
                if (RolesParents.TryGetValue(modPlugin.ModAssembly, out Transform p))
                {
                    foreach (RoleBehaviour roleBehaviour in modPlugin.Roles)
                    {
                        if (roleBehaviour.Role != RoleTypes.Crewmate && roleBehaviour.Role != RoleTypes.Impostor && roleBehaviour.Role != RoleTypes.CrewmateGhost && roleBehaviour.Role != RoleTypes.ImpostorGhost && GameOptionsManager.Instance.CurrentGameOptions.RoleOptions.GetChancePerGame(roleBehaviour.Role) > 0)
                        {
                            if (roleBehaviour.CustomRole() != null && roleBehaviour.CustomRole().Configuration.HideInLobby) continue;

                            GameObject.Instantiate<MatchInfoRolePanel>(__instance.MatchInfoRolePanelPrefab, p).SetPanel(roleBehaviour, GameOptionsManager.Instance.CurrentGameOptions.RoleOptions.GetNumPerGame(roleBehaviour.Role), GameOptionsManager.Instance.CurrentGameOptions.RoleOptions.GetChancePerGame(roleBehaviour.Role));
                        }
                    }
                }
            }

            if (RolesParents.TryGetValue(FungleApiPlugin.Plugin.ModAssembly, out Transform vanilla))
            {
                vanilla.gameObject.SetActive(true);
                __instance.rolesEnabledMessage.SetActive(vanilla.GetChildCount() == 0);
                if (!__instance.rolesEnabledMessage.activeSelf)
                {
                    __instance.MatchInfoRoleScroller.SetYBoundsMax(Mathf.Clamp(Mathf.Ceil((float)vanilla.GetChildCount() / 2f) + __instance.RoleEntryBoundsModifier, 0f, 999f));
                }
            }

            __instance.MatchInfoRoleMaskArea.material.SetInt(PlayerMaterial.MaskLayer, 50);
            __instance.matchInfoSettingsMaskArea.material.SetInt(PlayerMaterial.MaskLayer, 50);

            __instance.CreatePlayerEntries();

            return false;
        }
    }
}
