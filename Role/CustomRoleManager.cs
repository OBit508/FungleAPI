using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AmongUs.GameOptions;
using FungleAPI.LoadMod;
using FungleAPI.Patches;
using Il2CppInterop.Runtime.Injection;
using UnityEngine;
using Il2CppInterop.Runtime;
using FungleAPI.Translation;
using FungleAPI.Role;
using System.Reflection;
using FungleAPI.Role.Teams;

namespace FungleAPI.Roles
{
    public static class CustomRoleManager
    {
        internal static int id = 1;
        public static List<RoleBehaviour> AllRoles = new List<RoleBehaviour>();
        internal static List<(Type x1, ModPlugin x2, RoleTypes x3)> RolesToRegister = new List<(Type x1, ModPlugin x2, RoleTypes x3)>();
        public static RoleTypes RegisterRole(Type type)
        {
            id++;
            RoleTypes roleType = (RoleTypes)(id + 99);
            RolesToRegister.Add((type, ModPlugin.GetModPlugin(type.Assembly), roleType));
            return roleType;
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
        internal static CustomRoleBehaviour Register(Type type, ModPlugin plugin, RoleTypes roleType)
        {
            try
            {
                ClassInjector.RegisterTypeInIl2Cpp(type);
                CustomRoleBehaviour role = (CustomRoleBehaviour)new GameObject().AddComponent(Il2CppType.From(type)).DontDestroy();
                ICustomRole cRole = role as ICustomRole;
                cRole.Count = FungleAPIPlugin.Instance.Config.Bind<int>(cRole.RolePlugin.ModName + "-" + cRole.RoleName, "Count", 1);
                cRole.Chance = FungleAPIPlugin.Instance.Config.Bind<int>(cRole.RolePlugin.ModName + "-" + cRole.RoleName, "Chance", 100);
                role.name = cRole.RoleName.GetString();
                role.StringName = cRole.RoleName.StringName;
                role.BlurbName = cRole.RoleBlur.StringName;
                role.BlurbNameMed = cRole.RoleBlurMed.StringName;
                role.BlurbNameLong = cRole.RoleBlurLong.StringName;
                role.NameColor = cRole.RoleColor;
                role.Role = roleType;
                plugin.Roles.Add(role);
                AllRoles.Add(role);
                cRole.OnRegister?.Invoke();
                plugin.BasePlugin.Log.LogInfo("Registered Role " + cRole.RoleName.GetString() + ".");
                return role;
            }
            catch
            {
                FungleAPIPlugin.Instance.Log.LogError("Failed to register Role of the mod " + plugin.ModName + ".");
                return null;
            }
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
                return role.RoleB.CanSabotage;
            }
            return roleBehaviour.TeamType == RoleTeamTypes.Impostor;
        }
        public static bool CanKill(this RoleBehaviour roleBehaviour)
        {
            ICustomRole role = roleBehaviour as ICustomRole;
            if (role != null)
            {
                return role.RoleB.CanKill;
            }
            return roleBehaviour.TeamType == RoleTeamTypes.Impostor;
        }
    }
}
