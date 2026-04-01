using AmongUs.GameOptions;
using FungleAPI.Base.Roles;
using FungleAPI.Configuration;
using FungleAPI.Configuration.Attributes;
using FungleAPI.Configuration.Helpers;
using FungleAPI.Event;
using FungleAPI.Event.API;
using FungleAPI.Player;
using FungleAPI.PluginLoading;
using FungleAPI.Teams;
using FungleAPI.Translation;
using FungleAPI.Utilities;
using FungleAPI.Utilities.Prefabs;
using HarmonyLib;
using Il2CppInterop.Runtime;
using Sentry.Internal.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEngine;
using xCloud;

namespace FungleAPI.Role
{
    /// <summary>
    /// A class that helps the role system to work
    /// </summary>
    public static class CustomRoleManager
    {
        internal static List<CachedWaitingRole> WaitingToRegister = new List<CachedWaitingRole>();
        /// <summary>
        /// Returns the NeutralGhost role created by the API
        /// </summary>
        public static RoleBehaviour NeutralGhost => RoleExtensions.GetRole<NeutralGhost>();
        /// <summary>
        /// Returns all game roles
        /// </summary>
        public static readonly List<RoleBehaviour> AllRoles = new List<RoleBehaviour>();
        /// <summary>
        /// Returns all custom roles
        /// </summary>
        public static readonly List<ICustomRole> AllCustomRoles = new List<ICustomRole>();
        /// <summary>
        /// Update the fields of a role and the configs
        /// </summary>
        public static void UpdateRole(RoleBehaviour role)
        {
            if (role == null)
            {
                return;
            }
            ICustomRole customRole = role.CustomRole();
            if (customRole != null)
            {
                role.StringName = customRole.RoleName;
                role.BlurbName = customRole.RoleBlur;
                role.BlurbNameMed = customRole.RoleBlurMed;
                role.BlurbNameLong = customRole.RoleBlurLong;
                role.NameColor = customRole.RoleColor;
                role.AffectedByLightAffectors = customRole.IsAffectedByLightOnAirship;
                role.CanUseKillButton = customRole.UseVanillaKillButton;
                role.CanVent = customRole.CanUseVent;
                role.TasksCountTowardProgress = customRole.CompletedTasksCountForProgress;
                role.RoleScreenshot = customRole.Screenshot;
                role.RoleIconSolid = customRole.IconSolid;
                role.RoleIconWhite = customRole.IconWhite;
            }
            if (role.Player != null && role.Player.AmOwner)
            {
                RoleConfigManager.UpdateByRole(role);
            }
        }

        [EventRegister]
        internal static void CreateRoles(GameOpen gameOpen)
        {
            foreach (CachedWaitingRole cachedWaitingRole in WaitingToRegister)
            {
                cachedWaitingRole.Role = RegisterRole(cachedWaitingRole.Type, cachedWaitingRole.Plugin, cachedWaitingRole.Id);
            }
            WaitingToRegister = null;
        }
        internal static void OrganizeRoles(RoleManager roleManager)
        {
            RoleBehaviour[] roles = AllRoles.ToArray();
            AllRoles.Clear();
            FungleAPIPlugin.Plugin.Roles.AddRange(roleManager.AllRoles.ToArray());
            AllRoles.AddRange(FungleAPIPlugin.Plugin.Roles);
            AllRoles.AddRange(roles);
        }
        internal static RoleBehaviour RegisterRole(Type type, ModPlugin plugin, RoleTypes roleType)
        {
            ChangeableValue<RoleOptions> roleOptions = ICustomRole.Save[type];
            RoleBehaviour role = (RoleBehaviour)new GameObject().AddComponent(Il2CppType.From(type)).DontDestroy();
            ICustomRole customRole = role.CustomRole();
            roleOptions.Value.Options.AddRange(ConfigurationManager.RegisterAllOptions(type, ConfigFileType.RoleOptions, plugin));
            ConfigurationManager.InitializeRoleCountAndChances(type, plugin);
            role.name = type.Name;
            role.StringName = customRole.RoleName;
            role.BlurbName = customRole.RoleBlur;
            role.BlurbNameMed = customRole.RoleBlurMed;
            role.BlurbNameLong = customRole.RoleBlurLong;
            role.NameColor = customRole.RoleColor;
            role.AffectedByLightAffectors = customRole.IsAffectedByLightOnAirship;
            role.CanUseKillButton = customRole.UseVanillaKillButton;
            role.CanVent = customRole.CanUseVent;
            role.TasksCountTowardProgress = customRole.CompletedTasksCountForProgress;
            role.RoleScreenshot = customRole.Screenshot;
            role.RoleIconSolid = customRole.IconSolid;
            role.RoleIconWhite = customRole.IconWhite;
            role.Role = roleType;
            role.TeamType = customRole.Team == ModdedTeamManager.Impostors ? RoleTeamTypes.Impostor : RoleTeamTypes.Crewmate;
            if (customRole.IsGhostRole)
            {
                RoleManager.GhostRoles.Add(roleType);
            }
            return role;
        }
    }
}
