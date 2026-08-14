using AmongUs.GameOptions;
using AsmResolver.PE.DotNet.ReadyToRun;
using FungleAPI.Api;
using FungleAPI.Components;
using FungleAPI.Event;
using FungleAPI.Event.Vanilla;
using FungleAPI.Event.Vanilla.Player;
using FungleAPI.Extensions;
using FungleAPI.GameModes;
using FungleAPI.GameOptions;
using FungleAPI.GameOptions.Lobby;
using FungleAPI.GameOver;
using FungleAPI.Hud;
using FungleAPI.ModCompatibility.MiraSupport;
using FungleAPI.Modifiers;
using FungleAPI.PluginLoading;
using FungleAPI.Role.Utilities;
using FungleAPI.Teams;
using FungleAPI.Utilities;
using HarmonyLib;
using InnerNet;
using MonoMod.Cil;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using xCloud;

namespace FungleAPI.Role.Patches
{
    [HarmonyPatch(typeof(RoleManager))]
    internal static class RoleManagerPatch
    {
        public static bool waitingRegister = true;
        [HarmonyPatch("Awake")]
        [HarmonyPostfix]
        public static void AwakePostfix(RoleManager __instance)
        {
            if (waitingRegister)
            {
                FungleApiPlugin.Plugin.Roles.AddRange(RoleManager.Instance.AllRoles.ToArray());
                CustomRoleManager.AllRoles.AddRange(FungleApiPlugin.Plugin.Roles);
                CustomRoleManager.CreateRoles();

                if (MiraCompatibility.Instance != null)
                {
                    CustomRoleManager.AllRoles.AddRange(MiraCompatibility.Instance.CompleteRoleRegistration());
                }

                RoleManager.Instance.AllRoles = CustomRoleManager.AllRoles.ToIl2CppList();

                foreach (ModPlugin plugin in ModPluginManager.AllPlugins)
                {
                    plugin.Settings.Initialize(plugin);
                    foreach (ModdedTeam moddedTeam in plugin.Teams)
                    {
                        moddedTeam.Initialize(plugin);
                    }

                    if (plugin == FungleApiPlugin.Plugin) continue;

                    List<LobbyTab> tabs = plugin.FunglePlugin.LoadTabs(plugin);

                    if (tabs.FindAll(t => t.GetType() != typeof(GamemodeSettingsTab)).Count > 0)

                    OptionManager.LobbyTabs[plugin.ModAssembly] = tabs;
                }

                MiraCompatibility.Instance?.PopulateMiraLobbyTabs();

                waitingRegister = false;
            }
        }
        [HarmonyPatch("SetRole")]
        [HarmonyPrefix]
        public static bool SetRolePrefix(RoleManager __instance, PlayerControl targetPlayer, RoleTypes roleType)
        {
            if (EventManager.CallEvent(new BeforeSetRoleEvent(targetPlayer, roleType)).Cancelled) return false;

            if (!targetPlayer)
            {
                return false;
            }
            NetworkedPlayerInfo data = targetPlayer.Data;
            if (data == null)
            {
                Debug.LogError("It shouldn't be possible, but " + targetPlayer.name + " still doesn't have PlayerData during role selection.");
                return false;
            }
            if (data.Role)
            {
                RoleBehaviour role = data.Role;

                if (RoleManager.IsGhostRole(role.Role))
                {
                    targetPlayer.GetComponent<PlayerHelper>().LastDeadRole = role.Role;
                }

                if (role != null && targetPlayer.AmOwner)
                {
                    if (role.CanUseKillButton)
                    {
                        RoleConfigManager.KillConfig?.ResetButton();
                    }
                    if (role.CanSabotage())
                    {
                        RoleConfigManager.SabotageConfig?.ResetButton?.Invoke();
                    }
                    if (role.CanVent)
                    {
                        RoleConfigManager.VentConfig?.ResetButton?.Invoke();
                    }
                    RoleConfigManager.ReportConfig?.ResetButton?.Invoke();
                }
                role.Deinitialize(targetPlayer);
                GameObject.Destroy(role.gameObject);
            }
            RoleBehaviour roleBehaviour = GameObject.Instantiate<RoleBehaviour>(__instance.AllRoles.FirstOrDefault(r => r.Role == roleType), data.gameObject.transform);
            targetPlayer.Data.Role = roleBehaviour;
            targetPlayer.Data.RoleType = roleType;
            roleBehaviour.Initialize(targetPlayer);
            if (!RoleManager.IsGhostRole(roleType))
            {
                targetPlayer.Data.RoleWhenAlive = new Il2CppSystem.Nullable<RoleTypes>(roleType);
            }
            roleBehaviour.AdjustTasks(targetPlayer);
            if (roleBehaviour.IsDead && !targetPlayer.Data.IsDead)
            {
                targetPlayer.Die(DeathReason.Kill, false);
                return false;
            }
            if (!roleBehaviour.IsDead && targetPlayer.Data.IsDead)
            {
                targetPlayer.Revive();
            }
            CustomRoleManager.UpdateRole(roleBehaviour);
            if (targetPlayer.AmOwner)
            {
                if (roleBehaviour.CanUseKillButton || targetPlayer.AnyModifierForceKill())
                {
                    RoleConfigManager.KillConfig.InitializeButton();
                    targetPlayer.SetKillTimer(0.01f);
                }
                if (roleBehaviour.CanSabotage() || targetPlayer.AnyModifierForceSabotage())
                {
                    RoleConfigManager.SabotageConfig.InitializeButton?.Invoke();
                }
                if (roleBehaviour.CanVent || targetPlayer.AnyModifierForceVent())
                {
                    RoleConfigManager.VentConfig.InitializeButton?.Invoke();
                }
                RoleConfigManager.ReportConfig.InitializeButton?.Invoke();

                foreach (CustomAbilityButton customAbilityButton in HudHelper.Buttons.Values)
                {
                    customAbilityButton.Reset(true);
                }
            }

            EventManager.CallEvent(new AfterSetRoleEvent(targetPlayer, roleType));
            return false;
        }
        [HarmonyPrefix]
        [HarmonyPatch("AssignRoleOnDeath")]
        public static bool AssignRoleOnDeath(RoleManager __instance, [HarmonyArgument(0)] PlayerControl plr)
        {
            if (!plr || !plr.Data.IsDead)
            {
                return false;
            }
            ICustomRole role = plr.Data.Role.CustomRole();
            if (role == null)
            {
                return true;
            }
            role.AssignRoleOnDeath(plr);
            return false;
        }

        [HarmonyPatch("SelectRoles")]
        [HarmonyPrefix]
        public static bool SelectRolesPrefix(RoleManager __instance)
        {
            if (!GameManager.Instance.IsHideAndSeek())
            {
                GameModeManager.GetCurrentGameMode().SelectRoles(__instance);
                return false;
            }
            return true;
        }
    }
}
