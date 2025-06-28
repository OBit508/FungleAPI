using System;
using BepInEx;
using BepInEx.Configuration;
using BepInEx.Unity.IL2CPP;
using FungleAPI.Patches;
using FungleAPI.MonoBehaviours;
using HarmonyLib;
using Il2CppInterop.Runtime.Injection;
using FungleAPI.Roles;
using FungleAPI.LoadMod;
using System.Reflection;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using FungleAPI.Assets;
using BepInEx.Logging;
using FungleAPI.Rpc;
using System.Collections;
using AmongUs.GameOptions;
using FungleAPI.Role;
using FungleAPI.Role.Teams;

namespace FungleAPI.LoadMod
{
	[BepInProcess("Among Us.exe")]
	[BepInPlugin(ModId, "FungleAPI", ModV)]
	public class FungleAPIPlugin : BasePlugin
	{
        public const string ModId = "com.rafael.fungleapi";
        public const string ModV = "0.1.0";
        public Harmony Harmony { get; } = new Harmony(ModId);
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
                ModdedTeam.Crewmates = ModdedTeam.RegisterTeam(typeof(CrewmateTeam));
                ModdedTeam.Impostors = ModdedTeam.RegisterTeam(typeof(ImpostorTeam));
                ModdedTeam.Neutrals = ModdedTeam.RegisterTeam(typeof(NeutralTeam));
                CustomRpcManager.LoadModRpcs();
            }
            Harmony.PatchAll();
		}
        internal static ModPlugin plugin;
		public static ModPlugin Plugin 
        {
            get
            {
                if (plugin == null)
                {
                    plugin = new ModPlugin();
                    plugin.ModAssembly = Assembly.GetExecutingAssembly();
                    plugin.ModName = "Vanilla";
                    plugin.BasePlugin = Instance;
                    ModPlugin.AllPlugins.Add(plugin);
                }
                return plugin;
            }
        }
		[HarmonyPatch(typeof(AmongUsClient))]
		public class LoadAPIThings
		{
			[HarmonyPatch("CreatePlayer")]
			[HarmonyPostfix]
			private static void SyncRoles(AmongUsClient __instance)
			{
                if (__instance.AmHost)
                {
                    CustomRpcManager.RpcSyncAllRoleSettings();
                }
            }
            [HarmonyPatch("Awake")]
            [HarmonyPostfix]
            private static void LoadAll()
			{
				if (!allLoadded)
				{
                    foreach (LoadedAsset asset in LoadedAsset.Assets)
                    {
                        if (asset.GetAsset() != null)
                        {
                        }
                    }
                    foreach (RoleBehaviour role in RoleManager.Instance.AllRoles)
                    {
                        Plugin.Roles.Add(role);
                        CustomRoleManager.AllRoles.Add(role);
                    }
                    List<CustomRoleBehaviour> roleB = new List<CustomRoleBehaviour>();
                    foreach ((Type x1, ModPlugin x2, RoleTypes x3) pair in CustomRoleManager.RolesToRegister)
                    {
                        roleB.Add(CustomRoleManager.Register(pair.x1, pair.x2, pair.x3));
                    }
                    RoleManager.Instance.DontDestroy().AllRoles = CustomRoleManager.AllRoles.ToArray();
                    foreach (CustomRoleBehaviour role in roleB)
                    {
                        if (role.IsGhostRole)
                        {
                            RoleManager.GhostRoles.Add(role.Role);
                        }
                    }
                    if (loadAll != null)
					{
                        loadAll();
                    }
                    allLoadded = true;
                }
			}
			private static bool allLoadded;
		}
        public static void AddNewLoad(Action load)
		{
			Action action = loadAll;
			loadAll = new Action(delegate
			{
                load();
                action();
            });
		}
		internal static Action loadAll = new Action(delegate
		{
		});
	}
}
