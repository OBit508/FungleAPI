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
using Il2CppSystem.Text;
using InnerNet;
using FungleAPI.Translation;

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
                ClassInjector.RegisterTypeInIl2Cpp<RoleHelper>();
                ClassInjector.RegisterTypeInIl2Cpp<HerePointBehaviour>();
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
                    plugin = ModPlugin.Register(Instance);
                    plugin.ModName = "Vanilla";
                }
                return plugin;
            }
        }
		[HarmonyPatch(typeof(AmongUsClient))]
		public class LoadAPIThings
		{
			[HarmonyPatch("CreatePlayer")]
			[HarmonyPostfix]
			private static void SyncRoles(AmongUsClient __instance, [HarmonyArgument(0)] ClientData clientData)
			{
                if (__instance.AmHost && clientData.Id != __instance.HostId)
                {
                    CustomRpcManager.RpcSyncAllRoleSettings(clientData.Id);
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
                    (RoleTypes role, Type type) neutralRole = Plugin.Roles[0];
                    Plugin.Roles.Clear();
                    foreach (RoleBehaviour role in RoleManager.Instance.AllRoles)
                    {
                        CustomRoleManager.AllRoles.Add(role);
                        Plugin.Roles.Add((role.Role, role.GetType()));
                    }
                    Plugin.Roles.Add(neutralRole);
                    foreach ((Type x1, ModPlugin x2, RoleTypes x3) pair in CustomRoleManager.RolesToRegister)
                    {
                        CustomRoleManager.Register(pair.x1, pair.x2, pair.x3);
                    }
                    RoleManager.Instance.DontDestroy().AllRoles = CustomRoleManager.AllRoles.ToArray();
                    RoleManager.Instance.GetRole(RoleTypes.CrewmateGhost).StringName = Translator.GetOrCreate("Crewmate Ghost").AddTranslation(SupportedLangs.Brazilian, "Fantasma inocente").StringName;
                    RoleManager.Instance.GetRole(RoleTypes.ImpostorGhost).StringName = Translator.GetOrCreate("Impostor Ghost").AddTranslation(SupportedLangs.Brazilian, "Fantasma impostor").StringName;
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
