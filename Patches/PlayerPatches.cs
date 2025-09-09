using AmongUs.GameOptions;
using AsmResolver.PE.DotNet.ReadyToRun;
using BepInEx.Unity.IL2CPP.Utils.Collections;
using Epic.OnlineServices.Presence;
using FungleAPI.Components;
using FungleAPI.Networking;
using FungleAPI.Networking.RPCs;
using FungleAPI.Role;
using FungleAPI.Role.Teams;
using FungleAPI.Utilities;
using FungleAPI.Utilities.Assets;
using HarmonyLib;
using Hazel;
using Il2CppInterop.Runtime;
using Il2CppInterop.Runtime.InteropTypes.Arrays;
using Il2CppInterop.Runtime.Runtime;
using Il2CppSystem.Net;
using InnerNet;
using Rewired;
using Rewired.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using xCloud;
using static UnityEngine.GraphicsBuffer;

namespace FungleAPI.Patches
{
    [HarmonyPatch(typeof(PlayerControl))]
    internal static class PlayerPatches
    {
        internal static List<Il2CppSystem.Type> AllPlayerComponents = new List<Il2CppSystem.Type>();
        [HarmonyPatch("Start")]
        [HarmonyPostfix]
        public static void StartPostfix(PlayerControl __instance)
        {
            __instance.StartCoroutine(TrySendCreateFungleAPIClient(__instance).WrapToIl2Cpp());
            __instance.myTasks.Add(new GameObject("RoleHintText")
            {
                transform =
                {
                    parent = __instance.transform
                }
            }.AddComponent<RoleHintText>());
            foreach (Il2CppSystem.Type type in AllPlayerComponents)
            {
                __instance.gameObject.AddComponent(type);
            }
        }
        [HarmonyPrefix]
        [HarmonyPatch("RpcMurderPlayer")]
        public static bool PlayerControlMurderPrefix(PlayerControl __instance, [HarmonyArgument(0)] PlayerControl target, [HarmonyArgument(1)] bool didSucceed)
        {
            __instance.RpcCustomMurderPlayer(target, MurderResultFlags.Succeeded);
            return false;
        }
        public static System.Collections.IEnumerator TrySendCreateFungleAPIClient(PlayerControl player)
        {
            while (player.Data == null && player.Data.ClientId == -1)
            {
                yield return null;
            }
            if (!player.isDummy && !player.notRealPlayer && player.AmOwner)
            {
                CustomRpcManager.Instance<RpcAmModded>().Send(player, player.NetId);
            }
        }
    }
}
