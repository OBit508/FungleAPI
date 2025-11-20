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
using FungleAPI.GameOver;
using Il2CppSystem.Net.NetworkInformation;

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
        public static bool RpcEndGamePrefix(GameManager __instance, [HarmonyArgument(0)] GameOverReason endReason)
        {
            __instance.RpcEndGame(endReason.GetGameOver());
            return false;
        }
        [HarmonyPatch("GetDeadBody")]
        [HarmonyPrefix]
        public static bool GetDeadBodyPrefix(GameManager __instance, [HarmonyArgument(0)] RoleBehaviour impostorRole, ref DeadBody __result)
        {
            __result = __instance.deadBodyPrefab[impostorRole.Role == RoleTypes.Viper || impostorRole.CustomRole() != null && impostorRole.CustomRole().CreatedDeadBodyOnKill == DeadBodyType.Viper ? 1 : 0];
            return false;
        }
    }
}
