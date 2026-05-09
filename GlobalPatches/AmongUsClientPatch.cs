using AmongUs.Data;
using Assets.CoreScripts;
using BepInEx.Unity.IL2CPP.Utils;
using BepInEx.Unity.IL2CPP.Utils.Collections;
using FungleAPI.Components;
using FungleAPI.GameOptions;
using FungleAPI.ModCompatibility;
using FungleAPI.Networking;
using FungleAPI.PluginLoading;
using FungleAPI.Role;
using FungleAPI.Teams;
using FungleAPI.Translation;
using FungleAPI.Utilities;
using HarmonyLib;
using Hazel;
using InnerNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace FungleAPI.Patches
{
    [HarmonyPatch(typeof(AmongUsClient))]
    internal static class AmongUsClientPatch
    {
        [HarmonyPatch("Awake")]
        [HarmonyPostfix]
        public static void AwakePostfix(AmongUsClient __instance)
        {
            foreach (ModPlugin plugin in ModPluginManager.AllPlugins)
            {
                plugin.Settings.Initialize(plugin);
                foreach (ModdedTeam moddedTeam in plugin.Teams)
                {
                    moddedTeam.Initialize(plugin);
                }
            }
        }
        [HarmonyPatch("CreatePlayer")]
        [HarmonyPostfix]
        public static void CreatePlayerPostfix(AmongUsClient __instance, ClientData clientData)
        {
            if (!__instance.AmHost || __instance.HostId == clientData.Id || ReactorSupport.ReactorAssembly != null)
            {
                return;
            }
            SyncManager.RpcSyncEverything(clientData.Id);
        }
    }
}
