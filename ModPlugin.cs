using AmongUs.GameOptions;
using BepInEx;
using BepInEx.Configuration;
using BepInEx.Unity.IL2CPP;
using FungleAPI;
using FungleAPI.Attributes;
using FungleAPI.Components;
using FungleAPI.Configuration;
using FungleAPI.Freeplay;
using FungleAPI.Networking;
using FungleAPI.Networking.RPCs;
using FungleAPI.Patches;
using FungleAPI.Role;
using FungleAPI.Role.Teams;
using FungleAPI.Utilities;
using FungleAPI.Utilities.Assets;
using Il2CppInterop.Runtime;
using Il2CppInterop.Runtime.Injection;
using InnerNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.AddressableAssets;
using UnityEngine.UIElements;
using xCloud;

namespace FungleAPI
{
    public class ModPlugin
    {
        internal ModPlugin()
        {
        }
        public bool UseShipReference;
        internal static void Register(ModPlugin plugin, BasePlugin basePlugin)
        {
            plugin.ModAssembly = basePlugin.GetType().Assembly;
            plugin.ModName = plugin.ModAssembly.GetName().Name;
            plugin.RealName = plugin.ModName;
            plugin.BasePlugin = basePlugin;
            foreach (Type type in plugin.ModAssembly.GetTypes())
            {
                try
                {
                    if (type.GetCustomAttribute<RegisterTypeInIl2Cpp>() != null)
                    {
                        ClassInjector.RegisterTypeInIl2Cpp(type);
                    }
                    if (typeof(ModSettings).IsAssignableFrom(type) && type != typeof(ModSettings))
                    {
                        plugin.Settings = (ModSettings)Activator.CreateInstance(type);
                    }
                    if (typeof(ModFolderConfig).IsAssignableFrom(type) && type != typeof(ModFolderConfig))
                    {
                        plugin.FolderConfig = (ModFolderConfig)Activator.CreateInstance(type);
                    }
                    else if (typeof(CustomAbilityButton).IsAssignableFrom(type) && type != typeof(CustomAbilityButton))
                    {
                        CustomAbilityButton.RegisterButton(type, plugin);
                    }
                    else if (typeof(RoleBehaviour).IsAssignableFrom(type) && typeof(ICustomRole).IsAssignableFrom(type))
                    {
                        CustomRoleManager.RegisterRole(type, plugin);
                    }
                    else if (typeof(ModdedTeam).IsAssignableFrom(type) && type != typeof(ModdedTeam))
                    {
                        ModdedTeam.RegisterTeam(type, plugin);
                    }
                    else if (typeof(RpcHelper).IsAssignableFrom(type) && type != typeof(RpcHelper) && type != typeof(CustomRpc<>))
                    {
                        CustomRpcManager.RegisterRpc(type, plugin);
                    }
                    else if (typeof(PlayerComponent).IsAssignableFrom(type) && type != typeof(PlayerComponent))
                    {
                        ClassInjector.RegisterTypeInIl2Cpp(type);
                        PlayerPatches.AllPlayerComponents.Add(Il2CppType.From(type));
                    }
                    else if (typeof(DeadBodyComponent).IsAssignableFrom(type) && type != typeof(DeadBodyComponent))
                    {
                        ClassInjector.RegisterTypeInIl2Cpp(type);
                        DeadBodyHelper.AllBodyComponents.Add(Il2CppType.From(type));
                    }
                    else if (typeof(VentComponent).IsAssignableFrom(type) && type != typeof(VentComponent))
                    {
                        ClassInjector.RegisterTypeInIl2Cpp(type);
                        VentPatch.AllVentComponents.Add(Il2CppType.From(type));
                    }
                }
                catch (Exception ex)
                {
                    basePlugin.Log.LogError(ex);
                }
            }
        }
        public static ModPlugin GetModPlugin(Assembly assembly)
        {
            foreach (ModPlugin mod in AllPlugins)
            {
                if (mod.ModAssembly == assembly)
                {
                    return mod;
                }
            }
            return null;
        }
        public static ModPlugin RegisterMod(BasePlugin basePlugin, string modVersion, Action loadAssets = null, string ModName = null)
        {
            ModPlugin plugin = new ModPlugin();
            if (FungleAPIPlugin.Plugin != null)
            {
                Register(plugin, basePlugin);
                List<ModPlugin> sameNamePlugins = new List<ModPlugin>();
                if (ModName != null)
                {
                    plugin.ModName = ModName;
                    plugin.RealName = ModName;
                }
                AllPlugins.ForEach(new Action<ModPlugin>(delegate (ModPlugin pl)
                {
                    if (pl.RealName == plugin.RealName)
                    {
                        sameNamePlugins.Add(pl);
                    }
                }));
                if (sameNamePlugins.Count > 0)
                {
                    plugin.ModName += " (" + sameNamePlugins.Count + ")";
                }
                plugin.ModVersion = modVersion;
                AllPlugins.Add(plugin);
            }
            if (loadAssets != null)
            {
                FungleAPIPlugin.loadAssets += loadAssets;
            }
            plugin.LocalMod = new Mod(plugin);
            return plugin;
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
        public static ModPlugin GetByNameAndGUID(string modName, string modVersion)
        {
            return AllPlugins.FirstOrDefault(plugin => plugin.ModName == modName && plugin.ModVersion == modVersion);
        }
        internal string RealName;
        public string ModName;
        public string ModVersion;
        public Assembly ModAssembly;
        public BasePlugin BasePlugin;
        public List<RoleBehaviour> Roles = new List<RoleBehaviour>();
        public static List<ModPlugin> AllPlugins = new List<ModPlugin>();
        public ModSettings Settings = new ModSettings();
        public ModFolderConfig FolderConfig = new ModFolderConfig();
        public Mod LocalMod;
        public class Mod
        {
            public static void Update(ClientData client)
            {
                string reason = "";
                ClientData myClient = AmongUsClient.Instance.GetClient(AmongUsClient.Instance.ClientId);
                List<Mod> myMods = myClient.GetMods();
                if (client != myClient)
                {
                    List<Mod> mods = client.GetMods();
                    foreach (Mod mod in myMods)
                    {
                        if (!mods.Any(m => m.Equals(mod)))
                        {
                            reason += "Missing: " + mod.Name + " (" + mod.Version + ")";
                        }
                    }
                    foreach (Mod mod in mods)
                    {
                        if (!myMods.Any(m => m.Equals(mod)))
                        {
                            reason += "Host are Missing: " + mod.Name + " (" + mod.Version + ")";
                        }
                    }
                }
                if (reason.Length > 0)
                {
                    AmongUsClient.Instance.HandleDisconnect(DisconnectReasons.Custom, reason);
                }
            }
            public Mod(ModPlugin plugin)
            {
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
            public Mod(string version, string name, string realName, string guid)
            {
                Version = version;
                Name = name;
                RealName = realName;
                GUID = guid;
            }
            public override int GetHashCode()
            {
                return HashCode.Combine(GUID, Name);
            }
            public override bool Equals(object obj)
            {
                if (obj is Mod mod)
                {
                    return mod.Version == Version && mod.Name == Name && mod.GUID == GUID && mod.RealName == RealName;
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
