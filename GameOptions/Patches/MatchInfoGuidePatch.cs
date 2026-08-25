using AmongUs.GameOptions;
using FungleAPI.Api;
using FungleAPI.Components;
using FungleAPI.GameModes;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
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

        [HarmonyPatch(nameof(MatchInfoGuide.Awake))]
        [HarmonyPostfix]
        public static void AwakePostfix(MatchInfoGuide __instance)
        {
            Transform parent = __instance.transform.GetChild(0);

            parent.GetChild(5).GetComponent<TextMeshPro>().alignment = TextAlignmentOptions.Right;

            pluginChanger = GameObject.Instantiate(FungleAssets.PluginChangerPrefab, parent);
            pluginChanger.transform.localPosition = new Vector3(-1.45f, 1.4f, -1);
            pluginChanger.transform.localScale = Vector3.one * 0.3f;

            pluginChanger.Plugins = OptionManager.GetAllAssembliesWithTabs();

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

            int num = 0;
            foreach (RoleBehaviour roleBehaviour in DestroyableSingleton<RoleManager>.Instance.AllRoles)
            {
                if (roleBehaviour.Role != RoleTypes.Crewmate && roleBehaviour.Role != RoleTypes.Impostor && roleBehaviour.Role != RoleTypes.CrewmateGhost && roleBehaviour.Role != RoleTypes.ImpostorGhost && GameOptionsManager.Instance.CurrentGameOptions.RoleOptions.GetChancePerGame(roleBehaviour.Role) > 0)
                {
                    __instance.CreateRoleEntry(roleBehaviour);
                    num++;
                }
            }
            if (num == 0)
            {
                __instance.rolesEnabledMessage.SetActive(true);
            }
            __instance.MatchInfoRoleScroller.SetYBoundsMax(Mathf.Clamp(Mathf.Ceil((float)num / 2f) + __instance.RoleEntryBoundsModifier, 0f, 999f));
            __instance.MatchInfoRoleMaskArea.material.SetInt(PlayerMaterial.MaskLayer, 50);
            __instance.matchInfoSettingsMaskArea.material.SetInt(PlayerMaterial.MaskLayer, 50);
            __instance.CreatePlayerEntries();

            return false;
        }
    }
}
