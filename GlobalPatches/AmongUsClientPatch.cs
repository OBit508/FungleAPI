using AmongUs.Data;
using Assets.CoreScripts;
using BepInEx.Unity.IL2CPP.Utils;
using BepInEx.Unity.IL2CPP.Utils.Collections;
using FungleAPI.Api;
using FungleAPI.Components;
using FungleAPI.Extensions;
using FungleAPI.GameOptions;
using FungleAPI.GameOptions.Lobby;
using FungleAPI.ModCompatibility;
using FungleAPI.ModCompatibility.MiraSupport;
using FungleAPI.Networking;
using FungleAPI.PluginLoading;
using FungleAPI.Role;
using FungleAPI.Role.Patches;
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

namespace FungleAPI.GlobalPatches
{
    internal static class AmongUsClientPatch
    {
        public static Dictionary<int, KeyValuePair<string, string>> WrongModdeds = new Dictionary<int, KeyValuePair<string, string>>();
        [HarmonyPatch(typeof(AmongUsClient), nameof(AmongUsClient.CreatePlayer))]
        [HarmonyPostfix]
        public static void SyncSettings(AmongUsClient __instance, ClientData clientData)
        {
            if (__instance.HostId == clientData.Id) return;

            SyncManager.RpcSyncEverything(clientData.Id);
        }
        [HarmonyPatch(typeof(AmongUsClient), nameof(AmongUsClient.OnPlayerLeft))]
        [HarmonyPostfix]
        public static void RemoveFromList(AmongUsClient __instance, ClientData data)
        {
            if (WrongModdeds.ContainsKey(data.Id))
            {
                WrongModdeds.Remove(data.Id);
            }
        }
        [HarmonyPatch(typeof(InnerNetClient), nameof(InnerNetClient.JoinGame))]
        [HarmonyPostfix]
        public static void ResetModdedList(InnerNetClient __instance)
        {
            WrongModdeds.Clear();
        }
    }
}
