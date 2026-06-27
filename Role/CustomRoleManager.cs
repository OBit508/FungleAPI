using AmongUs.GameOptions;
using FungleAPI.Api;
using FungleAPI.Base.Roles;
using FungleAPI.Event;
using FungleAPI.Extensions;
using FungleAPI.GameOptions.Collections;
using FungleAPI.Player;
using FungleAPI.PluginLoading;
using FungleAPI.Role.Utilities;
using FungleAPI.Teams;
using FungleAPI.Translation;
using FungleAPI.Utilities;
using HarmonyLib;
using Il2CppInterop.Runtime;
using Il2CppInterop.Runtime.Injection;
using Sentry.Internal.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEngine;
using xCloud;
using static LetterTree;

namespace FungleAPI.Role
{
    /// <summary>
    /// Manage custom roles
    /// </summary>
    public static class CustomRoleManager
    {
        internal static uint LastRoleId = 19;
        internal static List<CachedWaitingRole> WaitingToRegister = new List<CachedWaitingRole>();
        internal static Dictionary<Type, RoleTypes> Types = new Dictionary<Type, RoleTypes>() 
        {
            { typeof(CrewmateRole), RoleTypes.Crewmate },
            { typeof(ImpostorRole), RoleTypes.Impostor },
            { typeof(ScientistRole), RoleTypes.Scientist },
            { typeof(GuardianAngelRole), RoleTypes.GuardianAngel },
            { typeof(EngineerRole), RoleTypes.Engineer },
            { typeof(ShapeshifterRole), RoleTypes.Shapeshifter },
            { typeof(CrewmateGhostRole), RoleTypes.CrewmateGhost },
            { typeof(ImpostorGhostRole), RoleTypes.ImpostorGhost },
            { typeof(NoisemakerRole), RoleTypes.Noisemaker },
            { typeof(PhantomRole), RoleTypes.Phantom },
            { typeof(TrackerRole), RoleTypes.Tracker },
            { typeof(ViperRole), RoleTypes.Viper },
            { typeof(DetectiveRole), RoleTypes.Detective },
        };
        /// <summary>
        /// Returns the NeutralGhost role created by the API
        /// </summary>
        public static RoleTypes NeutralGhost => GetRoleType<NeutralGhost>();
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
                RoleConfiguration roleConfiguration = customRole.Configuration;
                role.StringName = customRole.RoleName;
                role.BlurbName = customRole.RoleBlur;
                role.BlurbNameMed = customRole.RoleBlurMed;
                role.BlurbNameLong = customRole.RoleBlurLong;
                role.NameColor = customRole.RoleColor;
                role.AffectedByLightAffectors = roleConfiguration.IsAffectedByLightOnAirship;
                role.CanUseKillButton = roleConfiguration.UseVanillaKillButton;
                role.CanVent = roleConfiguration.CanUseVent;
                role.TasksCountTowardProgress = roleConfiguration.CompletedTasksCountForProgress;
                role.RoleScreenshot = roleConfiguration.Screenshot;
                role.RoleIconSolid = roleConfiguration.IconSolid;
                role.RoleIconWhite = roleConfiguration.IconWhite;
            }
            if (role.Player != null && role.Player.AmOwner)
            {
                RoleConfigManager.UpdateByRole(role);
            }
        }
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
            if (Types.TryGetValue(type, out RoleTypes roleTypes))
            {
                return roleTypes;
            }
            return default;
        }
        /// <summary>
        /// Returns a role type
        /// </summary>
        public static RoleTypes GetRoleType<T>() where T : RoleBehaviour
        {
            return GetRoleType(typeof(T));
        }
        /// <summary>
        /// Returns the plugin that registered this role
        /// </summary>
        public static ModPlugin GetRolePlugin(this RoleBehaviour role)
        {
            return GetRolePlugin(role.GetType()) ?? null;
        }
        /// <summary>
        /// Returns the plugin that registered this role
        /// </summary>
        public static ModPlugin GetRolePlugin<T>() where T : RoleBehaviour
        {
            return GetRolePlugin(typeof(T)) ?? null;
        }
        /// <summary>
        /// Returns the plugin that registered this role
        /// </summary>
        public static ModPlugin GetRolePlugin(Type type)
        {
            return ModPluginManager.AllPlugins.FirstOrDefault(p => p.Roles.Contains(GetRole(type)));
        }
        public static void RegisterRole(Type type, ModPlugin modPlugin)
        {
            if (WaitingToRegister == null)
            {
                throw new Exception("You can't register a Role when the RoleManager already loadded");
            }
            RoleTypes roleTypes = (RoleTypes)LastRoleId;
            WaitingToRegister.Add(new CachedWaitingRole(roleTypes, type, modPlugin));
            Types.Add(type, roleTypes);
            LastRoleId++;
            ClassInjector.RegisterTypeInIl2Cpp(type);
        }
        internal static void CreateRoles()
        {
            foreach (CachedWaitingRole cachedWaitingRole in WaitingToRegister)
            {
                RoleBehaviour roleBehaviour = RegisterRole(cachedWaitingRole.Type, cachedWaitingRole.Plugin, cachedWaitingRole.Id);
                cachedWaitingRole.Plugin.Roles.Add(roleBehaviour);
                AllCustomRoles.Add(roleBehaviour.CustomRole());
                AllRoles.Add(roleBehaviour);
            }
            WaitingToRegister = null;
        }
        internal static RoleBehaviour RegisterRole(Type type, ModPlugin plugin, RoleTypes roleType)
        {
            RoleBehaviour role = (RoleBehaviour)new GameObject().AddComponent(Il2CppType.From(type)).DontDestroy();
            ICustomRole customRole = role.CustomRole();

            RoleOptionCollection roleOptions = new RoleOptionCollection(customRole);
            ICustomRole.Save[roleType] = roleOptions;
            roleOptions.Initialize(type, plugin);

            RoleConfiguration roleConfiguration = customRole.Configuration;
            role.name = type.Name;
            role.StringName = customRole.RoleName;
            role.BlurbName = customRole.RoleBlur;
            role.BlurbNameMed = customRole.RoleBlurMed;
            role.BlurbNameLong = customRole.RoleBlurLong;
            role.NameColor = customRole.RoleColor;
            role.AffectedByLightAffectors = roleConfiguration.IsAffectedByLightOnAirship;
            role.CanUseKillButton = roleConfiguration.UseVanillaKillButton;
            role.CanVent = roleConfiguration.CanUseVent;
            role.TasksCountTowardProgress = roleConfiguration.CompletedTasksCountForProgress;
            role.RoleScreenshot = roleConfiguration.Screenshot;
            role.RoleIconSolid = roleConfiguration.IconSolid;
            role.RoleIconWhite = roleConfiguration.IconWhite;
            role.Role = roleType;
            role.TeamType = (RoleTeamTypes)customRole.Team.TeamId;
            if (role.IsDead)
            {
                RoleManager.GhostRoles.Add(roleType);
            }
            return role;
        }
    }
}
