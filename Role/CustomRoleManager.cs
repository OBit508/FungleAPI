using AmongUs.GameOptions;
using BepInEx.Core.Logging.Interpolation;
using FungleAPI.LoadMod;
using FungleAPI.MonoBehaviours;
using FungleAPI.Patches;
using FungleAPI.Role;
using FungleAPI.Role.Teams;
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

namespace FungleAPI.Roles
{
    public static class CustomRoleManager
    {
        public static RoleTypes NeutralGhost;
        public static List<RoleBehaviour> AllRoles = new List<RoleBehaviour>();
        internal static List<(Type x1, ModPlugin x2, RoleTypes x3)> RolesToRegister = new List<(Type x1, ModPlugin x2, RoleTypes x3)>();
        public static RoleTypes RegisterRole(Type type)
        {
            ICustomRole.id++;
            RoleTypes role = (RoleTypes)ICustomRole.id;
            if (typeof(RoleBehaviour).IsAssignableFrom(type) || typeof(ICustomRole).IsAssignableFrom(type))
            {
                ICustomRole.AllTypes.Add((type, role));
                RolesToRegister.Add((type, ModPlugin.GetModPlugin(type.Assembly), role));
                ClassInjector.RegisterTypeInIl2Cpp(type);
            }
            return role;
        }
        public static ICustomRole GetRole(RoleTypes type)
        {
            foreach (RoleBehaviour role in AllRoles)
            {
                if ((role as ICustomRole) != null && role.Role == type)
                {
                    return (role as ICustomRole);
                }
            }
            return null;
        }
        public static void MurderPlayer(PlayerControl killer, PlayerControl target)
        {
            var method = killer.Data.Role.GetType().GetMethod("MurderPlayer", BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
            if (method == null)
            {
                return;
            }
            var p = method.GetParameters();
            if (p.Length == 2 &&
                p[0].ParameterType == typeof(PlayerControl) &&
                p[1].ParameterType == typeof(CustomDeadBody))
            {
                method.Invoke(null, new object[] { target, target.GetBody() });
            }
        }
        internal static RoleBehaviour Register(Type type, ModPlugin plugin, RoleTypes roleType)
        {
            RoleBehaviour role = (RoleBehaviour)new GameObject().AddComponent(Il2CppType.From(type)).DontDestroy();
            ICustomRole cRole = role as ICustomRole;
            cRole.Count = plugin.BasePlugin.Config.Bind(cRole.RolePlugin.ModName + "-" + cRole.RoleName, "Count", 1);
            cRole.Chance = plugin.BasePlugin.Config.Bind(cRole.RolePlugin.ModName + "-" + cRole.RoleName, "Chance", 100);
            role.name = cRole.RoleName.GetString();
            role.StringName = cRole.RoleName;
            role.BlurbName = cRole.RoleBlur;
            role.BlurbNameMed = cRole.RoleBlurMed;
            role.BlurbNameLong = cRole.RoleBlurLong;
            role.NameColor = cRole.RoleColor;
            role.Role = roleType;
            plugin.Roles.Add(role);
            AllRoles.Add(role);
            if (cRole.CachedConfig.IsGhostRole)
            {
                RoleManager.GhostRoles.Add(roleType);
            }
            plugin.BasePlugin.Log.LogInfo("Registered Role " + cRole.RoleName.GetString() + ".");
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
            ICustomRole crole = role as ICustomRole;
            if (crole != null)
            {
                return crole.Team;
            }
            if (role.TeamType == RoleTeamTypes.Crewmate)
            {
                return ModdedTeam.Crewmates;
            }
            return ModdedTeam.Impostors;
        }
        public static bool CanSabotage(this RoleBehaviour roleBehaviour)
        {
            ICustomRole role = roleBehaviour as ICustomRole;
            if (role != null)
            {
                return role.CachedConfig.CanSabotage;
            }
            return roleBehaviour.TeamType == RoleTeamTypes.Impostor;
        }
        public static bool CanKill(this RoleBehaviour roleBehaviour)
        {
            ICustomRole role = roleBehaviour as ICustomRole;
            if (role != null)
            {
                return role.CachedConfig.CanKill;
            }
            return roleBehaviour.TeamType == RoleTeamTypes.Impostor;
        }
    }
}
