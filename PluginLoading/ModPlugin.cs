using BepInEx.Configuration;
using BepInEx.Unity.IL2CPP;
using FungleAPI.Base.Rpc;
using FungleAPI.Cosmetics;
using FungleAPI.Freeplay;
using FungleAPI.GameOptions;
using FungleAPI.GameOptions.Collections;
using FungleAPI.GameOptions.Lobby;
using FungleAPI.GameOver;
using FungleAPI.Role.Utilities;
using FungleAPI.Teams;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

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
        public List<BaseGameOver> GameOvers = new List<BaseGameOver>();
        public List<RpcHelper> RPCs = new List<RpcHelper>();
        public List<RoleBehaviour> Roles = new List<RoleBehaviour>();
        public List<ModdedTeam> Teams = new List<ModdedTeam>();
        public List<OptionCollection> OptionCollections = new List<OptionCollection>();


        public ModCosmetics Cosmetics;
        public ModSettings Settings;
        public ModFolderConfig FolderConfig;

        public BepInMod LocalMod;
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
