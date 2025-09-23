using AmongUs.Data;
using Assets.CoreScripts;
using BepInEx.Unity.IL2CPP.Utils.Collections;
using FungleAPI.Components;
using FungleAPI.Networking;
using FungleAPI.Networking.RPCs;
using FungleAPI.Role;
using FungleAPI.Translation;
using FungleAPI.Utilities;
using HarmonyLib;
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
            bool flag = false;
            foreach (ModPlugin plugin in ModPlugin.AllPlugins)
            {
                plugin.Settings.Initialize();
                if (plugin.UseShipReference)
                {
                    flag = true;
                }
            }
            if (flag)
            {
                __instance.StartCoroutine(Utilities.Prefabs.PrefabUtils.CoLoadShipPrefabs().WrapToIl2Cpp());
            }
        }
        [HarmonyPatch("CreatePlayer")]
        [HarmonyPostfix]
        public static void CreatePlayerPostfix(AmongUsClient __instance, [HarmonyArgument(0)] ClientData clientData)
        {
            if (!__instance.AmHost || clientData.Id == __instance.HostId)
            {
                return;
            }
            LobbyWarningText.nonModdedPlayers.Add(clientData, new Utilities.ChangeableValue<float>(5));
            CustomRpcManager.Instance<RpcSyncAllConfigs>().Send(null, PlayerControl.LocalPlayer.NetId, Hazel.SendOption.Reliable, clientData.Id);
        }
        public static DisconnectReasons FailedToSyncOptionsError = (DisconnectReasons)(-100);
    }
}
