using AmongUs.GameOptions;
using FungleAPI.Base.Roles;
using FungleAPI.Configuration;
using FungleAPI.Configuration.Attributes;
using FungleAPI.Configuration.Helpers;
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

namespace FungleAPI.Role
{
    /// <summary>
    /// 
    /// </summary>
    public static class CustomRoleManager
    {
        internal static Dictionary<Type, RoleTypes> RolesToRegister = new Dictionary<Type, RoleTypes>();
        /// <summary>
        /// 
        /// </summary>
        public static RoleBehaviour NeutralGhost => GetRole<NeutralGhost>();
        /// <summary>
        /// 
        /// </summary>
        public static readonly List<RoleBehaviour> AllRoles = new List<RoleBehaviour>();
        /// <summary>
        /// 
        /// </summary>
        public static readonly List<ICustomRole> AllCustomRoles = new List<ICustomRole>();
        /// <summary>
        /// 
        /// </summary>
        public static RoleBehaviour GetRole(Type type)
        {
            return AllRoles.FirstOrDefault(r => r.GetType() == type);
        }
        /// <summary>
        /// 
        /// </summary>
        public static T GetRole<T>() where T : RoleBehaviour
        {
            return GetRole(typeof(T)).SafeCast<T>();
        }
        /// <summary>
        /// 
        /// </summary>
        public static RoleTypes GetRoleType(Type type)
        {
            return GetRole(type).Role;
        }
        /// <summary>
        /// 
        /// </summary>
        public static RoleTypes GetRoleType<T>() where T : RoleBehaviour
        {
            return GetRoleType(typeof(T));
        }
        /// <summary>
        /// 
        /// </summary>
        public static ICustomRole CustomRole(this RoleBehaviour role)
        {
            return role as ICustomRole;
        }
        /// <summary>
        /// 
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
        /// 
        /// </summary>
        public static ModPlugin GetRolePlugin(this RoleBehaviour role)
        {
            ICustomRole customRole = role.CustomRole();
            if (customRole != null)
            {
                return ModPluginManager.GetModPlugin(role.GetType().Assembly);
            }
            return FungleAPIPlugin.Plugin;
        }
        /// <summary>
        /// 
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
        /// 
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
        /// 
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
        /// 
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
        /// 
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
        /// 
        /// </summary>
        public static bool CanVent(this RoleBehaviour roleBehaviour)
        {
            if (roleBehaviour.CustomRole() != null)
            {
                return roleBehaviour.CustomRole().CanUseVent;
            }
            return roleBehaviour.CanVent;
        }
        /// <summary>
        /// 
        /// </summary>
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
            (ChangeableValue<List<ModdedOption>> Options, ChangeableValue<RoleCountAndChance> CountAndChance) pair = ICustomRole.Save[type];
            RoleBehaviour role = (RoleBehaviour)new GameObject().AddComponent(Il2CppType.From(type)).DontDestroy();
            ICustomRole customRole = role.CustomRole();
            pair.Options.Value.AddRange(ConfigurationManager.RegisterAllOptions(type, plugin));
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
