using AmongUs.GameOptions;
using FungleAPI.Components;
using FungleAPI.Role.Teams;
using FungleAPI.Roles;
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

namespace FungleAPI.Patches
{
    [HarmonyPatch(typeof(GameManager))]
    internal static class GameManagerPatch
    {
        [HarmonyPatch("Awake")]
        [HarmonyPostfix]
        private static void OnAwake(GameManager __instance)
        {
            __instance.deadBodyPrefab?.gameObject.AddComponent<DeadBodyHelper>();
        }
    }
}
