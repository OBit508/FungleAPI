using AmongUs.Data;
using Assets.CoreScripts;
using BepInEx.Unity.IL2CPP.Utils;
using BepInEx.Unity.IL2CPP.Utils.Collections;
using FungleAPI.Components;
using FungleAPI.Configuration.Networking;
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
            foreach (ModdedTeam team in ModdedTeam.Teams)
            {
                team.Initialize(ModPluginManager.GetModPlugin(team.GetType().Assembly));
            }
            foreach (ModPlugin plugin in ModPlugin.AllPlugins)
            {
                plugin.Settings.Initialize(plugin);
                plugin.Options.AddRange(plugin.Settings.Options);
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
            Rpc<RpcSyncAllConfigs>.Instance.Send(PlayerControl.LocalPlayer, SendOption.Reliable, clientData.Id);
        }
    }
}
