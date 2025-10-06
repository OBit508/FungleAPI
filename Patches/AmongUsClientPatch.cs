using AmongUs.Data;
using Assets.CoreScripts;
using BepInEx.Unity.IL2CPP.Utils;
using BepInEx.Unity.IL2CPP.Utils.Collections;
using FungleAPI.Components;
using FungleAPI.Networking;
using FungleAPI.Networking.RPCs;
using FungleAPI.Role;
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
            if (clientData.Id == __instance.HostId)
            {
                return;
            }
            LobbyWarningText.nonModdedPlayers.Add(clientData, new Utilities.ChangeableValue<float>(5));
            if (__instance.AmHost)
            {
                __instance.StartCoroutine(SafeSend(new Action(delegate
                {
                    CustomRpcManager.Instance<RpcSyncAllConfigs>().Send(PlayerControl.LocalPlayer.NetId, Hazel.SendOption.Reliable, clientData.Id);
                    CustomRpcManager.Instance<RpcSendMods>().Send(__instance.GetClient(__instance.ClientId), PlayerControl.LocalPlayer.NetId, SendOption.Reliable, clientData.Id);
                })));
            }
            else
            {
                __instance.StartCoroutine(SafeSend(new Action(delegate
                {
                    CustomRpcManager.Instance<RpcSendMods>().Send(__instance.GetClient(__instance.ClientId), PlayerControl.LocalPlayer.NetId, SendOption.Reliable, clientData.Id);
                })));
            }
        }
        public static System.Collections.IEnumerator SafeSend(Action ac)
        {
            while (PlayerControl.LocalPlayer == null || PlayerControl.LocalPlayer.Data == null || PlayerControl.LocalPlayer.Data.ClientId == -1 || AmongUsClient.Instance.ClientId == -1)
            {
                yield return null;
            }
            ac();
        }
        public static DisconnectReasons FailedToSyncOptionsError = (DisconnectReasons)(-100);
    }
}
