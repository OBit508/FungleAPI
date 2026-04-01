using AmongUs.GameOptions;
using FungleAPI.Base.Roles;
using FungleAPI.Player;
using FungleAPI.PluginLoading;
using FungleAPI.Teams;
using FungleAPI.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FungleAPI.Role
{
    public static class RoleExtensions
    {
        /// <summary>
        /// Returns a role instance
        /// </summary>
        public static RoleBehaviour GetRole(Type type)
        {
            return CustomRoleManager.AllRoles.FirstOrDefault(r => r.GetType() == type);
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
            return ModPlugin.AllPlugins.FirstOrDefault(p => p.Roles.Contains(GetRole(type)));
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
            return RoleManager.IsImpostorRole(role.Role) ? ModdedTeamManager.Impostors : ModdedTeamManager.Crewmates;
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
    }
}
