using AmongUs.GameOptions;
using BepInEx;
using BepInEx.Configuration;
using BepInEx.Unity.IL2CPP;
using FungleAPI.Attributes;
using FungleAPI.Base.Rpc;
using FungleAPI.Components;
using FungleAPI.Cosmetics;
using FungleAPI.Freeplay;
using FungleAPI.GameMode;
using FungleAPI.GameOptions;
using FungleAPI.GameOptions.Collections;
using FungleAPI.GameOptions.Lobby;
using FungleAPI.GameOver;
using FungleAPI.Networking;
using FungleAPI.Patches;
using FungleAPI.Role.Utilities;
using FungleAPI.Teams;
using FungleAPI.Utilities;
using FungleAPI.Utilities.Assets;
using Il2CppInterop.Runtime;
using Il2CppInterop.Runtime.Injection;
using InnerNet;
using Sentry.Protocol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.AddressableAssets;
using UnityEngine.UIElements;
using xCloud;

namespace FungleAPI.PluginLoading
{
    /// <summary>
    /// A class used throughout the API that represents the registered mod
    /// </summary>
    public class ModPlugin
    {
        public string RealName;
        public string ModName;
        public string ModVersion;
        public string ModCredits;


        public Assembly ModAssembly;
        public BasePlugin BasePlugin;

        public ConfigEntry<byte> RulePreset;
        public bool HasRoles;

        public List<Type> AllTypes = new List<Type>();
        public List<LobbyTab> LobbyTabs = new List<LobbyTab>();
        public List<CustomGameMode> GameModes = new List<CustomGameMode>();
        public List<CustomGameOver> GameOvers = new List<CustomGameOver>();
        public List<RpcHelper> RPCs = new List<RpcHelper>();
        public List<RoleBehaviour> Roles = new List<RoleBehaviour>();
        public List<ModdedTeam> Teams = new List<ModdedTeam>();
        public List<OptionCollection> OptionCollections = new List<OptionCollection>();


        public ModCosmetics Cosmetics = new ModCosmetics();
        public ModSettings Settings = new ModSettings();
        public ModFolderConfig FolderConfig = new ModFolderConfig();


        public BepInMod LocalMod => BepInMod.GetMod(ModAssembly);
        internal ModPlugin()
        {
        }
        public Dictionary<ModdedTeam, List<RoleBehaviour>> GetTeamsAndRoles()
        {
            Dictionary<ModdedTeam, List<RoleBehaviour>> teams = new Dictionary<ModdedTeam, List<RoleBehaviour>>();
            if (Roles.Any(r => r.GetTeam() == ModdedTeamManager.Crewmates))
            {
                teams.Add(ModdedTeamManager.Crewmates, new List<RoleBehaviour>());
            }
            if (Roles.Any(r => r.GetTeam() == ModdedTeamManager.Impostors))
            {
                teams.Add(ModdedTeamManager.Impostors, new List<RoleBehaviour>());
            }
            if (Roles.Any(r => r.GetTeam() == ModdedTeamManager.Neutrals))
            {
                teams.Add(ModdedTeamManager.Neutrals, new List<RoleBehaviour>());
            }
            foreach (RoleBehaviour role in Roles)
            {
                ModdedTeam team = role.GetTeam();
                if (teams.TryGetValue(team, out List<RoleBehaviour> roles))
                {
                    roles.Add(role);
                }
                else
                {
                    teams[team] = new List<RoleBehaviour>() { role };
                }
            }
            return teams;
        }
    }
}
