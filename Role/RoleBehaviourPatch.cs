using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FungleAPI.Roles;
using UnityEngine;
using HarmonyLib;
using AmongUs.GameOptions;
using FungleAPI.Role.Teams;

namespace FungleAPI.Role
{
    [HarmonyPatch(typeof(RoleBehaviour))]
    public static class RoleBehaviourPatch
    {
        [HarmonyPrefix]
        [HarmonyPatch("TeamColor", MethodType.Getter)]
        public static bool GetTeamColor(RoleBehaviour __instance, ref Color __result)
        {
            ICustomRole role = __instance as ICustomRole;
            if (role != null)
            {
                if (role.CachedConfig.ShowTeamColor)
                {
                    __result = role.Team.TeamColor;
                }
                else
                {
                    __result = role.RoleColor;
                }
                return false;
            }
            return false;
        }
        [HarmonyPrefix]
        [HarmonyPatch("IsSimpleRole", MethodType.Getter)]
        public static bool GetSimpleRole(RoleBehaviour __instance, ref bool __result)
        {
            if (__instance as ICustomRole != null)
            {
                __result = true;
                return false;
            }
            return true;
        }
        [HarmonyPrefix]
        [HarmonyPatch("AffectedByLightAffectors", MethodType.Getter)]
        public static bool GetAffectedByLightAirShip(RoleBehaviour __instance, ref bool __result)
        {
            if (__instance as ICustomRole != null)
            {
                __result = (__instance as ICustomRole).CachedConfig.AffectedByLightOnAirship;
                return false;
            }
            return true;
        }
        [HarmonyPrefix]
        [HarmonyPatch("IsAffectedByComms", MethodType.Getter)]
        public static bool GetAffectedByComss(RoleBehaviour __instance, ref bool __result)
        {
            if (__instance as ICustomRole != null)
            {
                __result = (__instance as ICustomRole).CachedConfig.AffectedByComms;
                return false;
            }
            return true;
        }
        [HarmonyPrefix]
        [HarmonyPatch("CanUseKillButton", MethodType.Getter)]
        public static bool GetCanKill(RoleBehaviour __instance, ref bool __result)
        {
            if (__instance as ICustomRole != null)
            {
                __result = (__instance as ICustomRole).CachedConfig.UseVanillaKillButton;
                return false;
            }
            return true;
        }
        [HarmonyPrefix]
        [HarmonyPatch("CanVent", MethodType.Getter)]
        public static bool GetCanVent(RoleBehaviour __instance, ref bool __result)
        {
            if (__instance as ICustomRole != null)
            {
                __result = (__instance as ICustomRole).CachedConfig.CanUseVent;
                return false;
            }
            return true;
        }
        [HarmonyPrefix]
        [HarmonyPatch("TasksCountTowardProgress", MethodType.Getter)]
        public static bool GetTasksCountTowardProgress(RoleBehaviour __instance, ref bool __result)
        {
            if (__instance as ICustomRole != null)
            {
                __result = (__instance as ICustomRole).CachedConfig.TasksCountForProgress;
                return false;
            }
            return true;
        }
        [HarmonyPrefix]
        [HarmonyPatch("IsDead", MethodType.Getter)]
        public static bool GetIsDead(RoleBehaviour __instance, ref bool __result)
        {
            if (__instance as ICustomRole != null)
            {
                __result = (__instance as ICustomRole).CachedConfig.IsGhostRole;
                return false;
            }
            return true;
        }
        [HarmonyPrefix]
        [HarmonyPatch("MaxCount", MethodType.Getter)]
        public static bool GetMaxCount(RoleBehaviour __instance, ref int __result)
        {
            if (__instance as ICustomRole != null)
            {
                __result = (__instance as ICustomRole).Count.Value;
                return false;
            }
            return true;
        }
        [HarmonyPrefix]
        [HarmonyPatch("DefaultGhostRole", MethodType.Getter)]
        public static bool GetDefaultGhostRole(RoleBehaviour __instance, ref RoleTypes __result)
        {
            if (__instance as ICustomRole != null)
            { 
                __result = (__instance as ICustomRole).CachedConfig.GhostRole;
                return false;
            }
            return true;
        }
        [HarmonyPrefix]
        [HarmonyPatch("TeamType", MethodType.Getter)]
        public static bool GetTeamType(RoleBehaviour __instance, ref RoleTeamTypes __result)
        {
            if (__instance as ICustomRole != null)
            {
                
                __result = (__instance as ICustomRole).Team == ModdedTeam.Impostors ? RoleTeamTypes.Impostor : RoleTeamTypes.Crewmate;
                return false;
            }
            return true;
        }
    }
}
