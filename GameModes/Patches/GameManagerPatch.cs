using AmongUs.GameOptions;
using FungleAPI.Components;
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
using FungleAPI.GameOver;
using Il2CppSystem.Net.NetworkInformation;

namespace FungleAPI.GameModes.Patches
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
        public static bool RpcEndGamePrefix(GameManager __instance, GameOverReason endReason)
        {
            __instance.RpcEndGame(endReason.GetGameOver());
            return false;
        }
        [HarmonyPatch(nameof(GameManager.StartGame))]
        [HarmonyPrefix]
        public static void StartGamePrefix(GameManager __instance)
        {
            GameModeManager.GetCurrentGameMode().OnGameStart();
        }
        [HarmonyPatch(nameof(GameManager.EndGame))]
        [HarmonyPrefix]
        public static void EndGamePrefix(GameManager __instance)
        {
            GameModeManager.GetCurrentGameMode().OnGameEnd();
        }
        [HarmonyPatch(nameof(GameManager.FixedUpdate))]
        [HarmonyPrefix]
        public static void FixedUpdatePrefix(GameManager __instance)
        {
            GameModeManager.GetCurrentGameMode().FixedUpdate();
        }
        [HarmonyPatch(nameof(GameManager.OnPlayerDisconnect))]
        [HarmonyPrefix]
        public static void OnPlayerDisconnectPrefix(GameManager __instance, PlayerControl pc)
        {
            GameModeManager.GetCurrentGameMode().OnPlayerDisconnect(pc);
        }
    }
}
