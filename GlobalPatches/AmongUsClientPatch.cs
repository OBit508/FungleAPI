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
    [HarmonyPatch(typeof(AmongUsClient))]
    internal static class AmongUsClientPatch
    {
        [HarmonyPatch(nameof(AmongUsClient.CreatePlayer))]
        [HarmonyPostfix]
        public static void SyncEverything(AmongUsClient __instance, ClientData clientData)
        {
            if (__instance.HostId == clientData.Id || !__instance.AmHost) return;

            SyncManager.RpcSyncEverything(clientData.Id);
        }
    }
}
