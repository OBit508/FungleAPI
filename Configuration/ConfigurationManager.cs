using AmongUs.GameOptions;
using BepInEx.Configuration;
using Epic.OnlineServices;
using Epic.OnlineServices.RTC;
using FungleAPI.Configuration.Attributes;
using FungleAPI.Configuration.Helpers;
using FungleAPI.Configuration.Presets;
using FungleAPI.Networking;
using FungleAPI.Networking.RPCs;
using FungleAPI.PluginLoading;
using FungleAPI.Role;
using FungleAPI.Teams;
using FungleAPI.Utilities;
using HarmonyLib;
using Hazel;
using InnerNet;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using UnityEngine;
using xCloud;
using static Il2CppSystem.Linq.Expressions.Interpreter.CastInstruction.CastInstructionNoT;
using static PlayerMaterial;

namespace FungleAPI.Configuration
{
    /// <summary>
    /// A class that helps the configuration system to work
    /// </summary>
    public static class ConfigurationManager
    {
        public const string NullId = "null";
        public const string CurrentVersion = "1.0";
        public static List<RoleCountAndChance> RoleCountsAndChances = new List<RoleCountAndChance>();
        public static List<TeamCountAndPriority> TeamCountAndPriorities = new List<TeamCountAndPriority>();
        public static List<ConfigHelper> ConfigHelpers = new List<ConfigHelper>();
        public static List<ModdedOption> Options = new List<ModdedOption>();
        public static string FunglePath = Path.Combine(Application.persistentDataPath, "FungleAPI");
        /// <summary>
        /// Registers all properties with ModdedOption of the given Type in the given Plugin as settings
        /// </summary>
        public static List<ModdedOption> RegisterAllOptions(Type type, ModPlugin modPlugin)
        {
            List<ModdedOption> moddedOptions = new List<ModdedOption>();
            foreach (PropertyInfo propertyInfo in type.GetProperties())
            {
                ModdedOption moddedOption = propertyInfo.GetCustomAttribute<ModdedOption>();
                if (moddedOption != null)
                {
                    RegisterModdedOption(moddedOption, modPlugin, propertyInfo);
                    moddedOptions.Add(moddedOption);
                }
            }
            return moddedOptions;
        }
        /// <summary>
        /// Register the property using ModdedOption in the Plugin as a setting
        /// </summary>
        public static void RegisterModdedOption(ModdedOption moddedOption, ModPlugin plugin, PropertyInfo propertyInfo)
        {
            moddedOption.Initialize(propertyInfo, plugin);
            MethodInfo method = propertyInfo.GetGetMethod(true);
            if (method != null)
            {
                Options.Add(moddedOption);
                plugin.Options.Add(moddedOption);
                HarmonyHelper.Patches.Add(method, new Func<object>(moddedOption.GetReturnedValue));
                FungleAPIPlugin.Harmony.Patch(method, new HarmonyMethod(typeof(HarmonyHelper).GetMethod("GetPrefix", BindingFlags.Static | BindingFlags.Public)));
            }
        }
        public static void InitializeRoleCountAndChances(Type roleType, ModPlugin plugin)
        {
            RoleCountAndChance roleCountAndChance = ICustomRole.Save[roleType].CountAndChance.Value;
            roleCountAndChance.Initialize(plugin.BasePlugin.Config, plugin.ModName + " - " + roleType.FullName, roleType);
            RoleCountsAndChances.Add(roleCountAndChance);
            ConfigHelpers.Add(roleCountAndChance);
            plugin.RoleCountsAndChances.Add(roleCountAndChance);
        }
        public static void InitializeTeamCountAndPriority(ModdedTeam team, ModPlugin plugin)
        {
            team.CountAndPriority = new TeamCountAndPriority();
            team.CountAndPriority.Initialize(plugin.BasePlugin.Config, team, plugin.ModName + " - " + team.GetType().FullName);
            TeamCountAndPriorities.Add(team.CountAndPriority);
            ConfigHelpers.Add(team.CountAndPriority);
            plugin.TeamCountAndPriorities.Add(team.CountAndPriority);
        }
    }
}
