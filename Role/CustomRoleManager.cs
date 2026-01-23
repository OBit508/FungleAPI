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
using System.Reflection;
using System.Text;
using UnityEngine;

namespace FungleAPI.Role
{
    public static class CustomRoleManager
    {
        public static RoleBehaviour NeutralGhost => Instance<NeutralGhost>();
        public static List<RoleBehaviour> AllRoles = new List<RoleBehaviour>();
        public static List<ICustomRole> AllCustomRoles = new List<ICustomRole>();
        internal static int id = Enum.GetNames<RoleTypes>().Length + 20;
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
        public static RoleTypes GetType(Type type)
        {
            return RolesToRegister[type];
        }
        public static ICustomRole CustomRole(this RoleBehaviour role)
        {
            return role as ICustomRole;
        }
        public static ICustomRole GetRole(RoleTypes type)
        {
            return RoleManager.Instance.GetRole(type).CustomRole();
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
            RoleConfigManager.UpdateByRole(role);
        }
        internal static RoleBehaviour Register(Type type, ModPlugin plugin, RoleTypes roleType)
        {
            (ChangeableValue<List<ModdedOption>> Options, ChangeableValue<RoleCountAndChance> CountAndChance) pair = ICustomRole.Save[type];
            RoleBehaviour role = (RoleBehaviour)new GameObject().AddComponent(Il2CppType.From(type)).DontDestroy();
            ICustomRole customRole = role.CustomRole();
            InitializeRoleOptions(role, plugin);
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
        internal static void InitializeRoleOptions(RoleBehaviour obj, ModPlugin plugin)
        {
            Type type = obj.GetType();
            foreach (PropertyInfo property in type.GetProperties())
            {
                ModdedOption att = (ModdedOption)property.GetCustomAttribute(typeof(ModdedOption));
                if (att != null)
                {
                    att.Initialize(property);
                    MethodInfo method = property.GetGetMethod(true);
                    if (method != null)
                    {
                        ConfigurationManager.Options.Add(att);
                        plugin.Options.Add(att);
                        HarmonyHelper.Patches.Add(method, new Func<object>(att.GetReturnedValue));
                        FungleAPIPlugin.Harmony.Patch(method, new HarmonyMethod(typeof(HarmonyHelper).GetMethod("GetPrefix", BindingFlags.Static | BindingFlags.Public)));
                    }
                    if (obj.CustomRole() != null)
                    {
                        obj.CustomRole().Options.Add(att);
                    }
                }
            }
        }
        public static void CreateForRole(Il2CppSystem.Text.StringBuilder sb, RoleBehaviour role, Color? color = null)
        {
            if (color == null)
            {
                color = role.TeamColor;
            }
            sb.AppendLine(color.Value.ToTextColor() + FungleTranslation.YourRoleIsText.GetString() + "<b>" + role.NiceName + "</b>.</color>");
            sb.AppendLine("<size=70%>" + role.BlurbLong);
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
        public static RoleHintType GetHintType(this RoleBehaviour role)
        {
            if (role.CustomRole() != null)
            {
                return role.CustomRole().HintType;
            }
            return RoleHintType.TaskHint;
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
                return roleBehaviour.CustomRole().CanSabotage;
            }
            return roleBehaviour.TeamType == RoleTeamTypes.Impostor;
        }
        public static bool CanKill(this RoleBehaviour roleBehaviour)
        {
            if (roleBehaviour.CustomRole() != null)
            {
                return roleBehaviour.CustomRole().CanKill;
            }
            return roleBehaviour.CanUseKillButton;
        }
        public static bool UseKillButton(this RoleBehaviour roleBehaviour)
        {
            if (roleBehaviour.CustomRole() != null)
            {
                return roleBehaviour.CustomRole().UseVanillaKillButton;
            }
            return roleBehaviour.CanUseKillButton;
        }
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
