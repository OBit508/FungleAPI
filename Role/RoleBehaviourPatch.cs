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
            if (__instance as ICustomRole != null)
            {
                __result = (__instance as ICustomRole).RoleColor;
                return false;
            }
            return false;
        }
        [HarmonyPrefix]
        [HarmonyPatch("NameColor", MethodType.Getter)]
        public static bool GetNameColor(RoleBehaviour __instance, ref Color __result)
        {
            if (__instance as ICustomRole != null)
            {
                __result = (__instance as ICustomRole).RoleColor;
                return false;
            }
            return true;
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
                __result = (__instance as ICustomRole).RoleB.AffectedByLightOnAirship;
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
                __result = (__instance as ICustomRole).RoleB.AffectedByComms;
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
                __result = (__instance as ICustomRole).RoleB.UseVanillaKillButton;
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
                __result = (__instance as ICustomRole).RoleB.CanUseVent;
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
                __result = (__instance as ICustomRole).RoleB.TasksCountForProgress;
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
                __result = (__instance as ICustomRole).RoleB.GhostRole;
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
