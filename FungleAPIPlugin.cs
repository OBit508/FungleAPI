using System;
using BepInEx;
using BepInEx.Configuration;
using BepInEx.Unity.IL2CPP;
using FungleAPI.MonoBehaviours;
using HarmonyLib;
using Il2CppInterop.Runtime.Injection;
using FungleAPI.Roles;
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
using System.Diagnostics;
using BepInEx.Unity.IL2CPP.Utils;
using Unity.Services.Core.Internal;
using FungleAPI.Utilities;

namespace FungleAPI
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
                ClassInjector.RegisterTypeInIl2Cpp<PlayerHelper>();
                ClassInjector.RegisterTypeInIl2Cpp<HerePointBehaviour>();
            }
            Harmony.PatchAll();
            Harmony.PatchAllDerivedMethods(typeof(InnerNetObject), typeof(CustomRpcManager).GetMethod("Prefix", BindingFlags.Static | BindingFlags.Public));
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
                    IEnumerator delay()
                    {
                        yield return new WaitForSeconds(0.1f);
                        while (clientData.Character == null)
                        {
                        }
                        CustomRoleManager.RpcSyncSettings();
                    }
                    __instance.StartCoroutine(delay());
                }
            }
            [HarmonyPatch("Awake")]
            [HarmonyPostfix]
            private static void LoadAll()
			{
				if (!allLoadded)
				{
                    Plugin.Roles = RoleManager.Instance.AllRoles.Concat(Plugin.Roles).ToList();
                    foreach (KeyValuePair<Type, RoleTypes> pair in CustomRoleManager.RolesToRegister)
                    {
                        CustomRoleManager.Register(pair.Key, ModPlugin.GetModPlugin(pair.Key.Assembly), pair.Value);
                    }
                    RoleManager.Instance.DontDestroy().AllRoles = RoleManager.Instance.AllRoles.Concat(CustomRoleManager.AllRoles).ToArray();
                    RoleManager.Instance.GetRole(RoleTypes.CrewmateGhost).StringName = Translator.GetOrCreate("Crewmate Ghost").AddTranslation(SupportedLangs.Brazilian, "Fantasma inocente").StringName;
                    RoleManager.Instance.GetRole(RoleTypes.ImpostorGhost).StringName = Translator.GetOrCreate("Impostor Ghost").AddTranslation(SupportedLangs.Brazilian, "Fantasma impostor").StringName;
                    allLoadded = true;
                }
			}
			private static bool allLoadded;
		}
	}
}
