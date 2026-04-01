using AmongUs.GameOptions;
using BepInEx.Unity.IL2CPP;
using FungleAPI.Attributes;
using FungleAPI.Base.Roles;
using FungleAPI.Base.Rpc;
using FungleAPI.Components;
using FungleAPI.Configuration;
using FungleAPI.Configuration.Attributes;
using FungleAPI.Configuration.Helpers;
using FungleAPI.Cosmetics;
using FungleAPI.Event;
using FungleAPI.Freeplay;
using FungleAPI.GameOver;
using FungleAPI.Hud;
using FungleAPI.Networking;
using FungleAPI.Patches;
using FungleAPI.Player.Patches;
using FungleAPI.Role;
using FungleAPI.Teams;
using FungleAPI.Translation;
using FungleAPI.Utilities;
using FungleAPI.Utilities.Assets;
using HarmonyLib;
using Il2CppInterop.Runtime;
using Il2CppInterop.Runtime.Injection;
using Rewired.Utils.Classes.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UIElements.UIR;
using xCloud;

namespace FungleAPI.PluginLoading
{
    /// <summary>
    /// A class that helps the mod system to work
    /// </summary>
    public static class ModPluginManager
    {
        internal static void Register(ModPlugin plugin, BasePlugin basePlugin)
        {
            plugin.ModAssembly = basePlugin.GetType().Assembly;
            plugin.ModName = plugin.ModAssembly.GetName().Name;
            plugin.RealName = plugin.ModName;
            plugin.BasePlugin = basePlugin;
            HashSet<Type> visited = new HashSet<Type>();
            void AddTypes(Type type)
            {
                if (!visited.Add(type))
                {
                    return;
                }
                plugin.AllTypes.Add(type);
                foreach (Type t in type.GetNestedTypes(AccessTools.all))
                {
                    AddTypes(t);
                }
            }
            foreach (Type type in plugin.ModAssembly.GetTypes())
            {
                AddTypes(type);
            }
            EventManager.RegisterEvents(plugin);
        }
        public static void RegisterTypes(ModPlugin plugin)
        {
            foreach (Type type in plugin.AllTypes)
            {
                try
                {
                    if (type.GetCustomAttribute<FungleIgnore>() == null)
                    {
                        if (type.GetCustomAttribute<RegisterTypeInIl2Cpp>() != null)
                        {
                            ClassInjector.RegisterTypeInIl2Cpp(type);
                        }
                        if (typeof(ModSettings).IsAssignableFrom(type) && type != typeof(ModSettings))
                        {
                            plugin.Settings = (ModSettings)Activator.CreateInstance(type);
                        }
                        else if (typeof(ModFolderConfig).IsAssignableFrom(type))
                        {
                            plugin.FolderConfig = (ModFolderConfig)Activator.CreateInstance(type);
                        }
                        else if (typeof(CustomAbilityButton).IsAssignableFrom(type))
                        {
                            RegisterButton(type, plugin);
                        }
                        else if (typeof(RoleBehaviour).IsAssignableFrom(type) && typeof(ICustomRole).IsAssignableFrom(type))
                        {
                            RegisterRole(type, plugin);
                        }
                        else if (typeof(CustomGameOver).IsAssignableFrom(type))
                        {
                            RegisterGameOver(type, plugin);
                        }
                        else if (typeof(ModdedTeam).IsAssignableFrom(type))
                        {
                            RegisterTeam(type, plugin);
                        }
                        else if (typeof(RpcHelper).IsAssignableFrom(type))
                        {
                            RegisterRpc(type, plugin);
                        }
                        else if (typeof(PlayerComponent).IsAssignableFrom(type))
                        {
                            ClassInjector.RegisterTypeInIl2Cpp(type);
                            PlayerControlPatch.AllPlayerComponents.Add(Il2CppType.From(type));
                        }
                        else if (typeof(DeadBodyComponent).IsAssignableFrom(type))
                        {
                            ClassInjector.RegisterTypeInIl2Cpp(type);
                            DeadBodyHelper.AllBodyComponents.Add(Il2CppType.From(type));
                        }
                        else if (typeof(VentComponent).IsAssignableFrom(type))
                        {
                            ClassInjector.RegisterTypeInIl2Cpp(type);
                            VentPatch.AllVentComponents.Add(Il2CppType.From(type));
                        }
                        else if (typeof(VentComponent).IsAssignableFrom(type))
                        {
                            ClassInjector.RegisterTypeInIl2Cpp(type);
                            VentPatch.AllVentComponents.Add(Il2CppType.From(type));
                        }
                        else if (typeof(ModCosmetics).IsAssignableFrom(type))
                        {
                            plugin.Cosmetics = RegisterCosmetics(type, plugin);
                        }
                    }
                }
                catch (Exception ex)
                {
                    plugin.BasePlugin.Log.LogError(ex);
                }
            }
            EventManager.RegisterEvents(plugin);
        }
        public static ModCosmetics RegisterCosmetics(Type type, ModPlugin plugin)
        {
            ModCosmetics cosmetics = (ModCosmetics)Activator.CreateInstance(type);
            CosmeticManager.Add(cosmetics);
            plugin.BasePlugin.Log.LogInfo("Registered Cosmetics " + type.Name);
            return cosmetics;
        }
        public static RpcHelper RegisterRpc(Type type, ModPlugin plugin)
        {
            LastRpcId++;
            RpcHelper rpc = (RpcHelper)Activator.CreateInstance(type);
            rpc.RpcId = LastRpcId;
            CustomRpcManager.AllRpc.Add(rpc);
            plugin.BasePlugin.Log.LogInfo("Registered RPC " + type.Name);
            return rpc;
        }
        public static object RegisterTeam(Type type, ModPlugin plugin)
        {
            LastTeamId++;
            ModdedTeam team = (ModdedTeam)Activator.CreateInstance(type);
            team.TeamId = LastTeamId;
            plugin.Teams.Add(team);
            ModdedTeamManager.Teams.Add(type, team);
            ConfigurationManager.InitializeTeamCountAndPriority(team, plugin);
            team.CountData = ScriptableObject.CreateInstance<FloatGameSetting>().DontUnload();
            team.CountData.Type = OptionTypes.Float;
            team.CountData.Title = FungleTranslation.CountText;
            team.CountData.Increment = 1;
            team.CountData.ValidRange = new FloatRange(0, team.MaxCount);
            team.CountData.FormatString = null;
            team.CountData.ZeroIsInfinity = false;
            team.CountData.SuffixType = NumberSuffixes.None;
            team.CountData.OptionName = FloatOptionNames.Invalid;
            team.PriorityData = ScriptableObject.CreateInstance<FloatGameSetting>().DontUnload();
            team.PriorityData.Type = OptionTypes.Float;
            team.PriorityData.Title = FungleTranslation.TeamPriorityText;
            team.PriorityData.Increment = 1;
            team.PriorityData.ValidRange = new FloatRange(0, 500);
            team.PriorityData.FormatString = null;
            team.PriorityData.ZeroIsInfinity = false;
            team.PriorityData.SuffixType = NumberSuffixes.None;
            team.PriorityData.OptionName = FloatOptionNames.Invalid;
            plugin.BasePlugin.Log.LogInfo("Registered Team " + type.Name);
            return team;
        }
        public static CustomGameOver RegisterGameOver(Type type, ModPlugin plugin)
        {
            CustomGameOver gameOver = (CustomGameOver)Activator.CreateInstance(type);
            if (plugin == FungleAPIPlugin.Plugin)
            {
                gameOver.GameOverId = (int)gameOver.Reason;
            }
            else
            {
                gameOver.GameOverId = (int)GameOverManager.GetValidGameOver();
            }
            plugin.GameOvers.Add(gameOver);
            plugin.BasePlugin.Log.LogInfo("Registered GameOver " + type.Name + " Id: " + ((int)gameOver.Reason).ToString());
            GameOverManager.CustomGameOvers.Add(type, gameOver);
            return gameOver;
        }
        public static RoleTypes RegisterRole(Type type, ModPlugin plugin)
        {
            LastRoleId++;
            RoleTypes role = (RoleTypes)LastRoleId;
            CustomRoleManager.WaitingToRegister.Add(new CachedWaitingRole(role, type, plugin));
            ClassInjector.RegisterTypeInIl2Cpp(type);
            ICustomRole.Save.Add(type, new ChangeableValue<RoleOptions>(new RoleOptions()));
            return role;
        }
        public static CustomAbilityButton RegisterButton(Type type, ModPlugin plugin)
        {
            CustomAbilityButton button = (CustomAbilityButton)Activator.CreateInstance(type);
            HudHelper.Buttons.Add(type, button);
            plugin.BasePlugin.Log.LogInfo("Registered CustomButton " + type.Name);
            return button;
        }
        public static ModPlugin GetModPlugin(Assembly assembly)
        {
            foreach (ModPlugin mod in ModPlugin.AllPlugins)
            {
                if (mod.ModAssembly == assembly)
                {
                    return mod;
                }
            }
            return null;
        }
        public static ModPlugin RegisterMod(BasePlugin basePlugin, string modVersion, string ModName = null, string ModCredits = null)
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
                ModPlugin.AllPlugins.ForEach(new Action<ModPlugin>(delegate (ModPlugin pl)
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
                ModPlugin.AllPlugins.Add(plugin);
            }
            if (ModCredits == null)
            {
                ModCredits = "[" + plugin.RealName + " v" + plugin.ModVersion + "]";
            }
            plugin.ModCredits = ModCredits;
            plugin.LocalMod = new ModPlugin.Mod(plugin);
            plugin.PluginPreset = new Configuration.Presets.PluginPreset() { Plugin = plugin, CurrentPresetVersion = basePlugin.Config.Bind("Presets", "Current Version", ConfigurationManager.NullId) };
            if (plugin.PluginPreset.CurrentPresetVersion.Value == ConfigurationManager.NullId)
            {
                plugin.PluginPreset.CurrentPresetVersion.Value = ConfigurationManager.CurrentVersion;
            }
            plugin.PluginPreset.Initialize();
            return plugin;
        }
        public static ModPlugin GetByNameAndGUID(string modName, string modVersion)
        {
            return ModPlugin.AllPlugins.FirstOrDefault(plugin => plugin.ModName == modName && plugin.ModVersion == modVersion);
        }
        internal static int LastRpcId = int.MinValue;
        internal static int LastTeamId = int.MinValue;
        internal static int LastRoleId = 30;
    }
}
