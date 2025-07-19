using AmongUs.GameOptions;
using BepInEx.Core.Logging.Interpolation;
using FungleAPI;
using FungleAPI.MonoBehaviours;
using FungleAPI.Patches;
using FungleAPI.Role;
using FungleAPI.Role.Configuration;
using FungleAPI.Role.Teams;
using FungleAPI.Rpc;
using FungleAPI.Translation;
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
using static UnityEngine.GraphicsBuffer;

namespace FungleAPI.Roles
{
    public static class CustomRoleManager
    {
        public static void RpcSyncSettings(string text = null)
        {
            CustomRpcManager.GetInstance<RpcSyncAllRoleSettings>().Send(text, PlayerControl.LocalPlayer.NetId);
        }
        public static RoleTypes NeutralGhost => GetInstance<NeutralGhost>();
        public static List<RoleBehaviour> AllRoles = new List<RoleBehaviour>();
        internal static List<(Type x1, ModPlugin x2, RoleTypes x3)> RolesToRegister = new List<(Type x1, ModPlugin x2, RoleTypes x3)>();
        internal static int id = 10;
        internal static int gameOverId = 20;
        public static RoleTypes GetInstance<T>() where T : RoleBehaviour
        {
            foreach ((RoleTypes role, Type type) pair in ModPlugin.GetModPlugin(typeof(T).Assembly).Roles)
            {
                if (pair.type == typeof(T))
                {
                    return pair.role;
                }
            }
            return RoleTypes.Crewmate;
        }
        internal static RoleTypes RegisterRole(Type type, ModPlugin plugin)
        {
            id++;
            RoleTypes role = (RoleTypes)id;
            RolesToRegister.Add((type, plugin, role));
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
        public static bool DidWin(RoleBehaviour roleBehaviour, GameOverReason gameOverReason)
        {
            ICustomRole role = roleBehaviour.CustomRole();
            if (role != null)
            {
                if (role.Team == ModdedTeam.Neutrals)
                {
                    return !roleBehaviour.IsDead && role.Team.WinReason == gameOverReason;
                }
                return role.Team.WinReason == gameOverReason;
            }
            return roleBehaviour.DidWin(gameOverReason);
        }
        internal static RoleBehaviour Register(Type type, ModPlugin plugin, RoleTypes roleType)
        {
            RoleBehaviour role = (RoleBehaviour)new GameObject().AddComponent(Il2CppType.From(type)).DontDestroy();
            ICustomRole customRole = role.CustomRole();
            ICustomRole.Values.Add((plugin.BasePlugin.Config.Bind(plugin.ModName + "-" + type.Name, "Count", 1), plugin.BasePlugin.Config.Bind(plugin.ModName + "-" + type.Name, "Chance", 100), customRole.Configuration, roleType, type));
            RoleConfig config = customRole.CachedConfiguration;
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
        public static ModdedTeam GetTeam(this RoleBehaviour role)
        {
            if (role.CustomRole() != null)
            {
                return role.CustomRole().Team;
            }
            if (role.TeamType == RoleTeamTypes.Crewmate)
            {
                return ModdedTeam.Crewmates;
            }
            return ModdedTeam.Impostors;
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
