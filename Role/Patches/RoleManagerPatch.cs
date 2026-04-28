using AmongUs.GameOptions;
using AsmResolver.PE.DotNet.ReadyToRun;
using FungleAPI.Components;
using FungleAPI.Event;
using FungleAPI.GameMode;
using FungleAPI.GameOver;
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

namespace FungleAPI.Role.Patches
{
    [HarmonyPatch(typeof(RoleManager))]
    internal static class RoleManagerPatch
    {
        public static bool waitingRegister = true;
        [HarmonyPatch("Awake")]
        [HarmonyPrefix]
        public static bool AwakePrefix(RoleManager __instance)
        {
            if (!RoleManager._instance)
            {
                RoleManager._instance = __instance;
                if (__instance.DontDestroy)
                {
                    UnityEngine.Object.DontDestroyOnLoad(__instance.gameObject);
                }
            }
            else if (RoleManager._instance != __instance)
            {
                UnityEngine.Object.Destroy(__instance.gameObject);
            }
            if (waitingRegister)
            {
                FungleAPIPlugin.Plugin.Roles.AddRange(__instance.AllRoles.ToArray());
                CustomRoleManager.AllRoles.AddRange(FungleAPIPlugin.Plugin.Roles);
                CustomRoleManager.CreateRoles();
                __instance.AllRoles = CustomRoleManager.AllRoles.ToIl2CppList();
                waitingRegister = false;
            }
            return false;
        }
        [HarmonyPatch("SetRole")]
        [HarmonyPrefix]
        public static bool SetRolePrefix(RoleManager __instance, [HarmonyArgument(0)] PlayerControl targetPlayer, [HarmonyArgument(1)] RoleTypes roleType)
        {
            RoleBehaviour role = targetPlayer.Data.Role;
            if (role != null)
            {
                targetPlayer.GetComponent<PlayerHelper>().OldRole = __instance.GetRole(role.Role);
                if (role.CanUseKillButton)
                {
                    RoleConfigManager.KillConfig?.ResetButton?.Invoke();
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
                data.Role.Deinitialize(targetPlayer);
                GameObject.Destroy(data.Role.gameObject);
            }
            RoleBehaviour roleBehaviour = GameObject.Instantiate<RoleBehaviour>(__instance.AllRoles.FirstOrDefault(r => r.Role == roleType), data.gameObject.transform);
            targetPlayer.Data.Role = roleBehaviour;
            targetPlayer.Data.RoleType = roleType;
            roleBehaviour.Initialize(targetPlayer);
            if (roleType != RoleTypes.ImpostorGhost && roleType != RoleTypes.CrewmateGhost)
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
            if (roleBehaviour.CanUseKillButton)
            {
                RoleConfigManager.KillConfig.InitializeButton?.Invoke();
            }
            if (roleBehaviour.CanSabotage())
            {
                RoleConfigManager.SabotageConfig.InitializeButton?.Invoke();
            }
            if (roleBehaviour.CanVent)
            {
                RoleConfigManager.VentConfig.InitializeButton?.Invoke();
            }
            RoleConfigManager.ReportConfig.InitializeButton?.Invoke();
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
            if (GameManager.Instance.LogicRoleSelection is LogicRoleSelectionNormal)
            {
                GameModeManager.GetActiveGameMode().SelectRoles(__instance);
                return false;
            }
            return true;
        }
    }
}
