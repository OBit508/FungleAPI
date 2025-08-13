using AmongUs.GameOptions;
using BepInEx;
using BepInEx.Configuration;
using BepInEx.Core.Logging.Interpolation;
using BepInEx.Logging;
using BepInEx.Unity.IL2CPP;
using BepInEx.Unity.IL2CPP.Utils;
using FungleAPI.Assets;
using FungleAPI.Configuration;
using FungleAPI.MCIPatches;
using FungleAPI.MonoBehaviours;
using FungleAPI.Role;
using FungleAPI.Role.Teams;
using FungleAPI.Roles;
using FungleAPI.Rpc;
using FungleAPI.Translation;
using FungleAPI.Utilities;
using HarmonyLib;
using Hazel;
using Il2CppInterop.Runtime.Injection;
using Il2CppSystem.Text;
using InnerNet;
using Steamworks;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using Unity.Services.Core.Internal;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

namespace FungleAPI
{
	[BepInProcess("Among Us.exe")]
	[BepInPlugin(ModId, "FungleAPI", ModV)]
	public class FungleAPIPlugin : BasePlugin
	{
        public const string ModId = "com.rafael.fungleapi";
        public const string ModV = "0.1.0";
        public static Harmony Harmony = new Harmony(ModId);
        public static FungleAPIPlugin Instance;
		public override void Load()
		{
            Instance = this;
            if (Plugin != null)
            {
                ClassInjector.RegisterTypeInIl2Cpp<CustomConsole>();
                ClassInjector.RegisterTypeInIl2Cpp<SpriteAnimator>();
                ClassInjector.RegisterTypeInIl2Cpp<CustomDeadBody>();
                ClassInjector.RegisterTypeInIl2Cpp<Updater>();
                ClassInjector.RegisterTypeInIl2Cpp<PlayerAnimator>();
                ClassInjector.RegisterTypeInIl2Cpp<CustomVent>();
                ClassInjector.RegisterTypeInIl2Cpp<HerePointBehaviour>();
                ClassInjector.RegisterTypeInIl2Cpp<RpcPair>();
            }
            Harmony.PatchAll();
            SceneManager.add_sceneLoaded(new Action<Scene, LoadSceneMode>(delegate (Scene scene, LoadSceneMode _)
            {
                if (!allLoadded && scene.name == "MainMenu")
                {
                    Plugin.Roles = RoleManager.Instance.AllRoles.Concat(Plugin.Roles).ToList();
                    foreach (KeyValuePair<Type, RoleTypes> pair in CustomRoleManager.RolesToRegister)
                    {
                        CustomRoleManager.Register(pair.Key, ModPlugin.GetModPlugin(pair.Key.Assembly), pair.Value);
                    }
                    RoleManager.Instance.DontDestroy().AllRoles = RoleManager.Instance.AllRoles.Concat(CustomRoleManager.AllRoles).ToArray();
                    RoleManager.Instance.GetRole(RoleTypes.CrewmateGhost).StringName = Translator.GetOrCreate("Crewmate Ghost").AddTranslation(SupportedLangs.Brazilian, "Fantasma inocente").StringName;
                    RoleManager.Instance.GetRole(RoleTypes.ImpostorGhost).StringName = Translator.GetOrCreate("Impostor Ghost").AddTranslation(SupportedLangs.Brazilian, "Fantasma impostor").StringName;
                    MCIUtils.TryPatchSwitchTo();
                    allLoadded = true;
                }
            }));
        }
        private static bool allLoadded;
        internal static ModPlugin plugin;
		public static ModPlugin Plugin 
        {
            get
            {
                if (plugin == null)
                {
                    plugin = new ModPlugin();
                    ModPlugin.Register(plugin, Instance);
                    plugin.ModName = "Vanilla";
                }
                return plugin;
            }
        }
        [HarmonyPatch(typeof(AmongUsClient), "CreatePlayer")]
        internal static class SyncOptions
        {
            public static void Postfix(AmongUsClient __instance, [HarmonyArgument(0)] ClientData clientData)
            {
                StackTrace trace = new StackTrace();
                if (clientData.Id != __instance.HostId && __instance.AmHost && MCIUtils.GetClient(clientData.Id) == null && !trace.ToString().Contains("_CreatePlayerInstanceEnumerator"))
                {
                    RpcPair pair = CustomRpcManager.CreateRpcPair(PlayerControl.LocalPlayer.NetId, SendOption.Reliable, clientData.Id);
                    foreach (RoleBehaviour role in RoleManager.Instance.AllRoles)
                    {
                        if (role.CustomRole() != null)
                        {
                            pair.AddRpc(CustomRpcManager.GetInstance<RpcSyncSeetings>(), role.CustomRole());
                        }
                    }
                    pair.SendPair();
                }
            }
        }
	}
}
