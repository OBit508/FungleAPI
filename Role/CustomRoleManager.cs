using AmongUs.GameOptions;
using FungleAPI.Base.Roles;
using FungleAPI.Configuration;
using FungleAPI.Configuration.Attributes;
using FungleAPI.Configuration.Helpers;
using FungleAPI.Player;
using FungleAPI.PluginLoading;
using FungleAPI.Teams;
using FungleAPI.Translation;
using FungleAPI.Utilities;
using HarmonyLib;
using Il2CppInterop.Runtime;
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
        internal static Dictionary<Type, RoleTypes> RolesToRegister = new Dictionary<Type, RoleTypes>();
        /// <summary>
        /// Returns the NeutralGhost role created by the API
        /// </summary>
        public static RoleBehaviour NeutralGhost => GetRole<NeutralGhost>();
        /// <summary>
        /// Returns all game roles
        /// </summary>
        public static readonly List<RoleBehaviour> AllRoles = new List<RoleBehaviour>();
        /// <summary>
        /// Returns all custom roles
        /// </summary>
        public static readonly List<ICustomRole> AllCustomRoles = new List<ICustomRole>();
        /// <summary>
        /// Returns a role instance
        /// </summary>
        public static RoleBehaviour GetRole(Type type)
        {
            return AllRoles.FirstOrDefault(r => r.GetType() == type);
        }
        /// <summary>
        /// Returns a role instance
        /// </summary>
        public static T GetRole<T>() where T : RoleBehaviour
        {
            return GetRole(typeof(T)).SafeCast<T>();
        }
        /// <summary>
        /// Returns a role type
        /// </summary>
        public static RoleTypes GetRoleType(Type type)
        {
            return GetRole(type).Role;
        }
        /// <summary>
        /// Returns a role type
        /// </summary>
        public static RoleTypes GetRoleType<T>() where T : RoleBehaviour
        {
            return GetRoleType(typeof(T));
        }
        /// <summary>
        /// Converts a RoleBehavior into an ICustomRole
        /// </summary>
        public static ICustomRole CustomRole(this RoleBehaviour role)
        {
            return role as ICustomRole;
        }
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
        /// <summary>
        /// Returns the plugin that registered this role
        /// </summary>
        public static ModPlugin GetRolePlugin(this RoleBehaviour role)
        {
            return ModPlugin.AllPlugins.FirstOrDefault(p => p.Roles.Contains(role));
        }
        /// <summary>
        /// Returns the created dead body type
        /// </summary>
        public static DeadBodyType GetCreatedDeadBody(this RoleBehaviour role)
        {
            if (role.CustomRole() != null)
            {
                return role.CustomRole().CreatedDeadBodyOnKill;
            }
            if (role.SafeCast<ViperRole>() != null)
            {
                return DeadBodyType.Viper;
            }
            return DeadBodyType.Normal;
        }
        /// <summary>
        /// Returns the role hint type
        /// </summary>
        public static RoleHintType GetHintType(this RoleBehaviour role)
        {
            if (role.CustomRole() != null)
            {
                return role.CustomRole().HintType;
            }
            return RoleHintType.TaskHint;
        }
        /// <summary>
        /// Returns the role team
        /// </summary>
        public static ModdedTeam GetTeam(this RoleBehaviour role)
        {
            if (role.CustomRole() != null)
            {
                return role.CustomRole().Team;
            }
            return RoleManager.IsImpostorRole(role.Role) ? ModdedTeam.Impostors : ModdedTeam.Crewmates;
        }
        /// <summary>
        /// Returns if the role can sabotage
        /// </summary>
        public static bool CanSabotage(this RoleBehaviour roleBehaviour)
        {
            if (roleBehaviour.CustomRole() != null)
            {
                return roleBehaviour.CustomRole().CanSabotage;
            }
            return roleBehaviour.TeamType == RoleTeamTypes.Impostor;
        }
        /// <summary>
        /// Returns if the role can kill
        /// </summary>
        public static bool CanKill(this RoleBehaviour roleBehaviour)
        {
            if (roleBehaviour.CustomRole() != null)
            {
                return roleBehaviour.CustomRole().CanKill;
            }
            return roleBehaviour.CanUseKillButton;
        }
        /// <summary>
        /// Returns if the role can use the vanilla kill button
        /// </summary>
        public static bool UseKillButton(this RoleBehaviour roleBehaviour)
        {
            if (roleBehaviour.CustomRole() != null)
            {
                return roleBehaviour.CustomRole().UseVanillaKillButton;
            }
            return roleBehaviour.CanUseKillButton;
        }
        /// <summary>
        /// Returns if the role can vent
        /// </summary>
        public static bool CanVent(this RoleBehaviour roleBehaviour)
        {
            if (roleBehaviour.CustomRole() != null)
            {
                return roleBehaviour.CustomRole().CanUseVent;
            }
            return roleBehaviour.CanVent;
        }
        public static void AppendHint(RoleBehaviour roleBehaviour, Il2CppSystem.Text.StringBuilder stringBuilder)
        {
            RoleBaseHelper roleBaseHelper = roleBehaviour.SafeCast<RoleBaseHelper>();
            if (roleBaseHelper != null)
            {
                roleBaseHelper.AppendHint(stringBuilder);
                return;
            }
            roleBehaviour.AppendTaskHint(stringBuilder);
        }
        internal static RoleBehaviour Register(Type type, ModPlugin plugin, RoleTypes roleType)
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
            role.TeamType = customRole.Team == ModdedTeam.Impostors ? RoleTeamTypes.Impostor : RoleTeamTypes.Crewmate;
            AllRoles.Add(role);
            AllCustomRoles.Add(customRole);
            plugin.Roles.Add(role);
            if (customRole.IsGhostRole)
            {
                RoleManager.GhostRoles.Add(roleType);
            }
            return role;
        }
    }
}
