using AmongUs.GameOptions;
using BepInEx.Configuration;
using Epic.OnlineServices;
using Epic.OnlineServices.RTC;
using FungleAPI.Configuration.Attributes;
using FungleAPI.Configuration.Helpers;
using FungleAPI.Configuration.Presets;
using FungleAPI.Networking;
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
using UnityEngine.UI;
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
        public static List<ModdedOption> Options = new List<ModdedOption>();
        public static List<ConfigHelper> ConfigHelpers = new List<ConfigHelper>();
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
        public static void SerializeOptions(MessageWriter messageWriter)
        {
            messageWriter.WritePacked(Options.Count);
            foreach (ModdedOption moddedOption in Options)
            {
                messageWriter.WriteOption(moddedOption);
                messageWriter.Write(moddedOption.localValue.Value);
            }
        }
        public static void DeserializeOptions(MessageReader messageReader)
        {
            int optionCount = messageReader.ReadPackedInt32();
            for (int i = 0; i < optionCount; i++)
            {
                ModdedOption moddedOption = messageReader.ReadOption();
                moddedOption.onlineValue = messageReader.ReadString();
            }
        }
        public static void SerializeRoleOptions(MessageWriter messageWriter)
        {
            messageWriter.WritePacked(CustomRoleManager.AllCustomRoles.Count);
            foreach (ICustomRole customRole in CustomRoleManager.AllCustomRoles)
            {
                RoleOptions roleOptions = customRole.RoleOptions;
                messageWriter.WriteConfigHelper(roleOptions);
                messageWriter.Write(roleOptions.Compact());
            }
        }
        public static void DeserializeRoleOptions(MessageReader messageReader)
        {
            int roleOptionCount = messageReader.ReadPackedInt32();
            for (int i = 0; i < roleOptionCount; i++)
            {
                RoleOptions roleOptions = messageReader.ReadConfigHelper<RoleOptions>();
                roleOptions.Decompact(messageReader.ReadString(), false);
            }
        }
        public static void SerializeTeamOptions(MessageWriter messageWriter)
        {
            messageWriter.WritePacked(ModdedTeam.Teams.Count);
            foreach (ModdedTeam team in ModdedTeam.Teams)
            {
                messageWriter.WriteConfigHelper(team.TeamOptions);
                messageWriter.Write(team.TeamOptions.Compact());
            }
        }
        public static void DeserializeTeamOptions(MessageReader messageReader)
        {
            int teamOptionCount = messageReader.ReadPackedInt32();
            for (int i = 0; i < teamOptionCount; i++)
            {
                TeamOptions teamOptions = messageReader.ReadConfigHelper<TeamOptions>();
                teamOptions.Decompact(messageReader.ReadString(), false);
            }
        }
        public static void InitializeRoleCountAndChances(Type roleType, ModPlugin plugin)
        {
            RoleOptions roleCountAndChance = ICustomRole.Save[roleType].Value;
            roleCountAndChance.Initialize(plugin.BasePlugin.Config, plugin.ModName + " - " + roleType.FullName);
            plugin.RoleOptions.Add(roleCountAndChance);
            ConfigHelpers.Add(roleCountAndChance);
        }
        public static void InitializeTeamCountAndPriority(ModdedTeam team, ModPlugin plugin)
        {
            team.TeamOptions = new TeamOptions();
            team.TeamOptions.Initialize(plugin.BasePlugin.Config, team, plugin.ModName + " - " + team.GetType().FullName);
            plugin.TeamOptions.Add(team.TeamOptions);
            ConfigHelpers.Add(team.TeamOptions);
        }
    }
}
