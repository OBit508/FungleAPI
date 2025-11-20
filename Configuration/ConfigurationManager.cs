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
using FungleAPI.Role.Teams;
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

namespace FungleAPI.Configuration
{
    public static class ConfigurationManager
    {
        public const string NullId = "Nenhuma versão ainda :D";
        public const string CurrentVersion = "1.0";
        public static List<RoleCountAndChance> RoleCountsAndChances = new List<RoleCountAndChance>();
        public static List<TeamCountAndPriority> TeamCountAndPriorities = new List<TeamCountAndPriority>();
        public static List<ModdedOption> Options = new List<ModdedOption>();
        public static string FunglePath = Path.Combine(Application.persistentDataPath, "FungleAPI");
        public static void InitializeRoleCountAndChances(Type roleType, ModPlugin plugin)
        {
            RoleCountAndChance roleCountAndChance = ICustomRole.Save[roleType].CountAndChance.Value;
            roleCountAndChance.Initialize(plugin.BasePlugin.Config, plugin.ModName + " - " + roleType.FullName, roleType);
            RoleCountsAndChances.Add(roleCountAndChance);
            plugin.RoleCountsAndChances.Add(roleCountAndChance);
        }
        public static void InitializeTeamCountAndPriority(ModdedTeam team, ModPlugin plugin)
        {
            team.CountAndPriority = new TeamCountAndPriority();
            team.CountAndPriority.Initialize(plugin.BasePlugin.Config, team, plugin.ModName + " - " + team.GetType().FullName);
            TeamCountAndPriorities.Add(team.CountAndPriority);
            plugin.TeamCountAndPriorities.Add(team.CountAndPriority);
        }
    }
}
