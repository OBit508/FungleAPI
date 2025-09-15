using AmongUs.GameOptions;
using FungleAPI.Components;
using FungleAPI.Role.Teams;
using FungleAPI.Role;
using FungleAPI.Networking;
using FungleAPI.Utilities;
using HarmonyLib;
using Hazel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using BepInEx.Unity.IL2CPP.Utils.Collections;

namespace FungleAPI.Patches
{
    [HarmonyPatch(typeof(GameManager))]
    internal static class GameManagerPatch
    {
        [HarmonyPatch("Awake")]
        [HarmonyPrefix]
        public static void AwakePrefix(GameManager __instance)
        {
            foreach (DeadBody deadBody in __instance.deadBodyPrefab)
            {
                deadBody.gameObject.AddComponent<DeadBodyHelper>();
            }
        }
        [HarmonyPatch("RpcEndGame")]
        [HarmonyPrefix]
        public static bool RpcEndGamePrefix(GameManager __instance, [HarmonyArgument(0)] GameOverReason endReason, [HarmonyArgument(1)] bool showAd)
        {
            __instance.ShouldCheckForGameEnd = false;
            __instance.logger.Info(string.Format("Endgame for {0}", endReason), null);
            MessageWriter messageWriter = AmongUsClient.Instance.StartEndGame();
            messageWriter.Write(false);
            messageWriter.Write((byte)endReason);
            messageWriter.Write(showAd);
            AmongUsClient.Instance.FinishEndGame(messageWriter);
            return false;
        }
    }
}
