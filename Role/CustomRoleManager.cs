using AmongUs.GameOptions;
using BepInEx.Core.Logging.Interpolation;
using FungleAPI;
using FungleAPI.Configuration;
using FungleAPI.Components;
using FungleAPI.Role;
using FungleAPI.Role.Teams;
using FungleAPI.Networking;
using FungleAPI.Translation;
using FungleAPI.Utilities;
using HarmonyLib;
using Hazel;
using Il2CppInterop.Generator.Extensions;
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
using xCloud;
using static Il2CppSystem.Reflection.RuntimePropertyInfo;
using static UnityEngine.GraphicsBuffer;

namespace FungleAPI.Role
{
    public static class CustomRoleManager
    {
        public static RoleBehaviour NeutralGhost => Instance<NeutralGhost>();
        public static List<RoleBehaviour> AllRoles = new List<RoleBehaviour>();
        public static List<ICustomRole> AllCustomRoles = new List<ICustomRole>();
        internal static int id = Enum.GetNames<RoleTypes>().Length;
        internal static int gameOverId = Enum.GetNames<GameOverReason>().Length;
        internal static Dictionary<Type, RoleTypes> RolesToRegister = new Dictionary<Type, RoleTypes>();
        public static T Instance<T>() where T : RoleBehaviour
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
        public static int CaculeCountByChance(this RoleBehaviour role, IRoleOptionsCollection roleOptions)
        {
            int count = 0;
            for (int i = 0; i < roleOptions.GetNumPerGame(role.Role); i++)
            {
                if (new System.Random().Next(0, 100) < roleOptions.GetChancePerGame(role.Role))
                {
                    count++;
                }
            }
            return count;
        }
        internal static RoleBehaviour Register(Type type, ModPlugin plugin, RoleTypes roleType)
        {
            RoleBehaviour role = (RoleBehaviour)new GameObject().AddComponent(Il2CppType.From(type)).DontDestroy();
            ICustomRole customRole = role.CustomRole();
            RoleConfig config = customRole.Configuration;
            config.Configs = ConfigurationManager.InitializeConfigs(role);
            ConfigurationManager.PatchRoleConfig(type, config);
            ConfigurationManager.InitializeRoleChanceAndCount(config, type, plugin);
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
            role.RoleScreenshot = config.Screenshot;
            role.Role = roleType;
            role.TeamType = customRole.Team == ModdedTeam.Impostors ? RoleTeamTypes.Impostor : RoleTeamTypes.Crewmate;
            AllRoles.Add(role);
            AllCustomRoles.Add(customRole);
            plugin.Roles.Add(role);
            if (customRole.Configuration.IsGhostRole)
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
            if (roleBehaviour.CustomRole() != null)
            {
                return roleBehaviour.CustomRole().Configuration.WinReason.Contains(gameOverReason) && (roleBehaviour.GetTeam() != ModdedTeam.Neutrals || roleBehaviour.GetTeam() == ModdedTeam.Neutrals && !roleBehaviour.Player.Data.IsDead);
            }
            if (roleBehaviour.GetTeam() == ModdedTeam.Neutrals)
            {
                return !roleBehaviour.Player.Data.IsDead && roleBehaviour.GetTeam().WinReason.Contains(gameOverReason);
            }
            return roleBehaviour.GetTeam().WinReason.Contains(gameOverReason);
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
                return roleBehaviour.CustomRole().Configuration.CanSabotage;
            }
            return roleBehaviour.TeamType == RoleTeamTypes.Impostor;
        }
        public static bool CanKill(this RoleBehaviour roleBehaviour)
        {
            if (roleBehaviour.CustomRole() != null)
            {
                return roleBehaviour.CustomRole().Configuration.CanKill;
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
                return roleBehaviour.CustomRole().Configuration.UseVanillaKillButton;
            }
            return roleBehaviour.CanUseKillButton;
        }
        public static bool CanVent(this RoleBehaviour roleBehaviour)
        {
            if (roleBehaviour.CustomRole() != null)
            {
                return roleBehaviour.CustomRole().Configuration.CanVent;
            }
            return roleBehaviour.CanVent;
        }
        public static bool CanDoTasks(this RoleBehaviour roleBehaviour)
        {
            if (roleBehaviour.CustomRole() != null)
            {
                return roleBehaviour.CustomRole().Configuration.CanDoTasks;
            }
            return roleBehaviour.GetTeam() == ModdedTeam.Crewmates;
        }
    }
}
