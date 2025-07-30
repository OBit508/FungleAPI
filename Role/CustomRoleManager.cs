using AmongUs.GameOptions;
using BepInEx.Core.Logging.Interpolation;
using FungleAPI;
using FungleAPI.Configuration;
using FungleAPI.MonoBehaviours;
using FungleAPI.Role;
using FungleAPI.Role.Teams;
using FungleAPI.Rpc;
using FungleAPI.Translation;
using FungleAPI.Utilities;
using HarmonyLib;
using Il2CppInterop.Runtime;
using Il2CppInterop.Runtime.Injection;
using MonoMod.Cil;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using static Il2CppSystem.Reflection.RuntimePropertyInfo;
using static UnityEngine.GraphicsBuffer;

namespace FungleAPI.Roles
{
    public static class CustomRoleManager
    {
        public static RoleBehaviour NeutralGhost => GetInstance<NeutralGhost>();
        public static List<RoleBehaviour> AllRoles = new List<RoleBehaviour>();
        internal static int id = 10;
        internal static int gameOverId = 20;
        internal static System.Collections.Generic.Dictionary<Type, RoleTypes> RolesToRegister = new System.Collections.Generic.Dictionary<Type, RoleTypes>();
        public static T GetInstance<T>() where T : RoleBehaviour
        {
            foreach (RoleBehaviour role in RoleManager.Instance.AllRoles)
            {
                if (role.GetType() == typeof(T))
                {
                    return role.SafeCast<T>();
                }
            }
            return null;
        }
        public static RoleTypes GetType<T>() where T : RoleBehaviour
        {
            return RolesToRegister[typeof(T)];
        }
        internal static RoleTypes RegisterRole(Type type, ModPlugin plugin)
        {
            id++;
            RoleTypes role = (RoleTypes)id;
            RolesToRegister.Add(type, role);
            ClassInjector.RegisterTypeInIl2Cpp(type);
            return role;
        }
        public static ICustomRole CustomRole(this RoleBehaviour role)
        {
            return role as ICustomRole;
        }
        public static ICustomRole GetRole(RoleTypes type)
        {
            return RoleManager.Instance.GetRole(type).CustomRole();
        }
        public static GameOverReason GetValidGameOver()
        {
            gameOverId++;
            return (GameOverReason)gameOverId;
        }
        internal static RoleBehaviour Register(Type type, ModPlugin plugin, RoleTypes roleType)
        {
            RoleBehaviour role = (RoleBehaviour)new GameObject().AddComponent(Il2CppType.From(type)).DontDestroy();
            ICustomRole customRole = role.CustomRole();
            ICustomRole.Values.Add((plugin.BasePlugin.Config.Bind(plugin.ModName + "-" + type.FullName, "Count", 1), plugin.BasePlugin.Config.Bind(plugin.ModName + "-" + type.FullName, "Chance", 100), customRole.Configuration, roleType, type));
            RoleConfig config = customRole.CachedConfiguration;
            config.Configs = ConfigurationManager.InitializeConfigs(role);
            role.name = type.Name;
            role.StringName = customRole.RoleName;
            role.BlurbName = customRole.RoleBlur;
            role.BlurbNameMed = customRole.RoleBlurMed;
            role.BlurbNameLong = customRole.RoleBlurLong;
            role.NameColor = customRole.RoleColor;
            role.AffectedByLightAffectors = config.AffectedByLightOnAirship;
            role.CanUseKillButton = config.UseVanillaKillButton;
            role.CanVent = config.CanVent;
            role.TasksCountTowardProgress = config.TasksCountForProgress;
            role.Role = roleType;
            role.InvokeMethod("Register", new Type[] { }, new object[] { });
            AllRoles.Add(role);
            plugin.Roles.Add(role);
            if (customRole.CachedConfiguration.IsGhostRole)
            {
                RoleManager.GhostRoles.Add(roleType);
            }
            return role;
        }
        public static ModPlugin GetRolePlugin(this RoleBehaviour role)
        {
            Assembly assembly = role.GetType().Assembly;
            foreach (ModPlugin plugin in ModPlugin.AllPlugins)
            {
                if (plugin.ModAssembly == assembly)
                {
                    return plugin;
                }
            }
            return FungleAPIPlugin.Plugin;
        }
        public static bool DidWin(RoleBehaviour roleBehaviour, GameOverReason gameOverReason)
        {
            ICustomRole role = roleBehaviour.CustomRole();
            if (role != null)
            {
                if (role.Team == ModdedTeam.Neutrals)
                {
                    return !roleBehaviour.IsDead && role.CachedConfiguration.WinReason == gameOverReason;
                }
                return role.CachedConfiguration.WinReason == gameOverReason;
            }
            return roleBehaviour.DidWin(gameOverReason);
        }
        public static ModdedTeam GetTeam(this RoleBehaviour role)
        {
            if (role.CustomRole() != null)
            {
                return role.CustomRole().Team;
            }
            return RoleManager.IsImpostorRole(role.Role) ? ModdedTeam.Impostors : ModdedTeam.Crewmates;
        }
        public static bool CanSabotage(this RoleBehaviour roleBehaviour)
        {
            if (roleBehaviour.CustomRole() != null)
            {
                return roleBehaviour.CustomRole().CachedConfiguration.CanSabotage;
            }
            return roleBehaviour.TeamType == RoleTeamTypes.Impostor;
        }
        public static bool CanKill(this RoleBehaviour roleBehaviour)
        {
            if (roleBehaviour.CustomRole() != null)
            {
                return roleBehaviour.CustomRole().CachedConfiguration.CanKill;
            }
            bool flag = roleBehaviour.CanUseKillButton;
            if (roleBehaviour.Role == RoleTypes.Phantom)
            {
                flag = !roleBehaviour.SafeCast<PhantomRole>().IsInvisible;
            }
            return flag && !roleBehaviour.Player.shapeshifting;
        }
        public static bool UseKillButton(this RoleBehaviour roleBehaviour)
        {
            if (roleBehaviour.CustomRole() != null)
            {
                return roleBehaviour.CustomRole().CachedConfiguration.UseVanillaKillButton;
            }
            return roleBehaviour.CanUseKillButton;
        }
        public static bool CanVent(this RoleBehaviour roleBehaviour)
        {
            if (roleBehaviour.CustomRole() != null)
            {
                return roleBehaviour.CustomRole().CachedConfiguration.CanVent;
            }
            return roleBehaviour.CanVent;
        }
    }
}
