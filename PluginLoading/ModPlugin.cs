using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using AmongUs.GameOptions;
using BepInEx;
using BepInEx.Configuration;
using BepInEx.Unity.IL2CPP;
using FungleAPI.Attributes;
using FungleAPI.Base.Rpc;
using FungleAPI.Components;
using FungleAPI.Configuration;
using FungleAPI.Configuration.Attributes;
using FungleAPI.Configuration.Helpers;
using FungleAPI.Configuration.Presets;
using FungleAPI.Cosmetics.Helpers;
using FungleAPI.Freeplay;
using FungleAPI.GameOver;
using FungleAPI.Networking;
using FungleAPI.Patches;
using FungleAPI.Role;
using FungleAPI.Teams;
using FungleAPI.Utilities;
using FungleAPI.Utilities.Assets;
using Il2CppInterop.Runtime;
using Il2CppInterop.Runtime.Injection;
using InnerNet;
using UnityEngine.AddressableAssets;
using UnityEngine.UIElements;
using xCloud;

namespace FungleAPI.PluginLoading
{
    public class ModPlugin
    {
        public static List<ModPlugin> AllPlugins = new List<ModPlugin>();
        internal ModPlugin()
        {
        }
        public Dictionary<ModdedTeam, List<RoleBehaviour>> GetTeamsAndRoles()
        {
            Dictionary<ModdedTeam, List<RoleBehaviour>> teams = new Dictionary<ModdedTeam, List<RoleBehaviour>>();
            if (Roles.Any(r => r.GetTeam() == ModdedTeam.Crewmates))
            {
                teams.Add(ModdedTeam.Crewmates, new List<RoleBehaviour>());
            }
            if (Roles.Any(r => r.GetTeam() == ModdedTeam.Impostors))
            {
                teams.Add(ModdedTeam.Impostors, new List<RoleBehaviour>());
            }
            if (Roles.Any(r => r.GetTeam() == ModdedTeam.Neutrals))
            {
                teams.Add(ModdedTeam.Neutrals, new List<RoleBehaviour>());
            }
            foreach (RoleBehaviour role in Roles)
            {
                ModdedTeam team = role.GetTeam();
                if (teams.ContainsKey(team))
                {
                    teams[team].Add(role);
                }
                else
                {
                    teams.Add(team, new List<RoleBehaviour>() { role });
                }
            }
            return teams;
        }
        public ConfigEntry<T> CreateConfig<T>(string Name, T value)
        {
            return BasePlugin.Config.Bind(ModName + " - Configs", Name, value);
        }
        internal string RealName;
        public string ModName;
        public string ModVersion;
        public string ModCredits;
        public Assembly ModAssembly;
        public BasePlugin BasePlugin;
        public PluginPreset PluginPreset;
        public List<Type> AllTypes = new List<Type>();
        public List<CustomGameOver> GameOvers = new List<CustomGameOver>();
        public List<RpcHelper> RPCs = new List<RpcHelper>();
        public List<RoleBehaviour> Roles = new List<RoleBehaviour>();
        public List<ModdedTeam> Teams = new List<ModdedTeam>();
        public List<ModdedOption> Options = new List<ModdedOption>();
        public List<RoleCountAndChance> RoleCountsAndChances = new List<RoleCountAndChance>();
        public List<TeamCountAndPriority> TeamCountAndPriorities = new List<TeamCountAndPriority>();
        public ModCosmetics Cosmetics = new ModCosmetics();
        public ModSettings Settings = new ModSettings();
        public ModFolderConfig FolderConfig = new ModFolderConfig();
        public Mod LocalMod;
        public class Mod
        {
            public Mod(ModPlugin plugin)
            {
                Plugin = plugin;
                Version = plugin.ModVersion;
                Name = plugin.ModName;
                RealName = plugin.RealName;
                if (plugin.BasePlugin != null)
                {
                    BepInPlugin p = plugin.BasePlugin.GetType().GetCustomAttribute<BepInPlugin>();
                    if (p != null)
                    {
                        GUID = p.GUID;
                    }
                }
            }
            public override int GetHashCode()
            {
                return HashCode.Combine(GUID, Name);
            }
            public override bool Equals(object obj)
            {
                if (obj is Mod mod)
                {
                    return mod.Version == Version && mod.Name == Name && mod.RealName == RealName;
                }
                return false;
            }
            public string Version;
            public string Name;
            public string RealName;
            public string GUID;
            public ModPlugin Plugin;
        }
    }
}
