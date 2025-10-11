using AmongUs.GameOptions;
using BepInEx.Unity.IL2CPP;
using FungleAPI.Attributes;
using FungleAPI.Components;
using FungleAPI.Configuration;
using FungleAPI.Configuration.Attributes;
using FungleAPI.Freeplay;
using FungleAPI.GameOver;
using FungleAPI.Hud;
using FungleAPI.Networking;
using FungleAPI.Networking.RPCs;
using FungleAPI.Patches;
using FungleAPI.Role;
using FungleAPI.Role.Teams;
using FungleAPI.Translation;
using FungleAPI.Utilities;
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
using static FungleAPI.PluginLoading.ModPlugin;

namespace FungleAPI.PluginLoading
{
    public static class ModPluginManager
    {
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
                    if (type.GetCustomAttribute<IgnoreOnFungleAutoRegister>() == null)
                    {
                        if (type.GetCustomAttribute<RegisterTypeInIl2Cpp>() != null)
                        {
                            ClassInjector.RegisterTypeInIl2Cpp(type);
                        }
                        if (typeof(ModSettings).IsAssignableFrom(type) && type != typeof(ModSettings))
                        {
                            plugin.Settings = (ModSettings)Activator.CreateInstance(type);
                        }
                        else if (typeof(ModFolderConfig).IsAssignableFrom(type) && type != typeof(ModFolderConfig))
                        {
                            plugin.FolderConfig = (ModFolderConfig)Activator.CreateInstance(type);
                        }
                        else if (typeof(CustomAbilityButton).IsAssignableFrom(type) && type != typeof(CustomAbilityButton))
                        {
                            RegisterButton(type, plugin);
                        }
                        else if (typeof(RoleBehaviour).IsAssignableFrom(type) && typeof(ICustomRole).IsAssignableFrom(type))
                        {
                            RegisterRole(type, plugin);
                        }
                        else if (typeof(CustomGameOver).IsAssignableFrom(type) && type != typeof(CustomGameOver))
                        {
                            RegisterGameOver(type, plugin);
                        }
                        else if (typeof(ModdedTeam).IsAssignableFrom(type) && type != typeof(ModdedTeam))
                        {
                            RegisterTeam(type, plugin);
                        }
                        else if (typeof(RpcHelper).IsAssignableFrom(type) && type != typeof(RpcHelper) && type != typeof(CustomRpc<>))
                        {
                            RegisterRpc(type, plugin);
                        }
                        else if (typeof(PlayerComponent).IsAssignableFrom(type) && type != typeof(PlayerComponent))
                        {
                            ClassInjector.RegisterTypeInIl2Cpp(type);
                            PlayerControlPatch.AllPlayerComponents.Add(Il2CppType.From(type));
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
                }
                catch (Exception ex)
                {
                    basePlugin.Log.LogError(ex);
                }
            }
        }
        public static RpcHelper RegisterRpc(Type type, ModPlugin plugin)
        {
            RpcHelper rpc = (RpcHelper)Activator.CreateInstance(type);
            CustomRpcManager.AllRpc.Add(rpc);
            plugin.BasePlugin.Log.LogInfo("Registered RPC " + type.Name);
            return rpc;
        }
        public static object RegisterTeam(Type type, ModPlugin plugin)
        {
            ModdedTeam team = (ModdedTeam)Activator.CreateInstance(type);
            plugin.Teams.Add(team);
            ModdedTeam.Teams.Add(team);
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
            team.PriorityData.Title = FungleTranslation.PriorityText;
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
            plugin.BasePlugin.Log.LogInfo("Registered GameOver " + type.Name + " Id: " + ((int)gameOver.Reason).ToString());
            GameOverManager.AllCustomGameOver.Add(gameOver);
            return gameOver;
        }
        public static RoleTypes RegisterRole(Type type, ModPlugin plugin)
        {
            CustomRoleManager.id++;
            RoleTypes role = (RoleTypes)CustomRoleManager.id;
            CustomRoleManager.RolesToRegister.Add(type, role);
            ClassInjector.RegisterTypeInIl2Cpp(type);
            ICustomRole.Save.Add(type, (new ChangeableValue<List<ModdedOption>>(new List<ModdedOption>()), new ChangeableValue<RoleCountAndChance>(new RoleCountAndChance()), new ChangeableValue<List<CustomAbilityButton>>(new List<CustomAbilityButton>())));
            return role;
        }
        public static CustomAbilityButton RegisterButton(Type type, ModPlugin plugin)
        {
            CustomAbilityButton button = (CustomAbilityButton)Activator.CreateInstance(type);
            CustomAbilityButton.Buttons.Add(type, button);
            plugin.BasePlugin.Log.LogInfo("Registered CustomButton " + type.Name);
            return button;
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
        public static ModPlugin RegisterMod(BasePlugin basePlugin, string modVersion, Action loadAssets = null, string ModName = null, string ModCredits = null)
        {
            ModPlugin plugin = new ModPlugin();
            if (FungleAPIPlugin.Plugin != null)
            {
                ModPluginManager.Register(plugin, basePlugin);
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
            if (ModCredits == null)
            {
                ModCredits = plugin.RealName;
            }
            plugin.ModCredits = ModCredits;
            if (loadAssets != null)
            {
                FungleAPIPlugin.loadAssets += loadAssets;
            }
            plugin.LocalMod = new Mod(plugin);
            return plugin;
        }
        public static ModPlugin GetByNameAndGUID(string modName, string modVersion)
        {
            return AllPlugins.FirstOrDefault(plugin => plugin.ModName == modName && plugin.ModVersion == modVersion);
        }
    }
}
