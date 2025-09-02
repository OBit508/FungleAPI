using AmongUs.GameOptions;
using AsmResolver.PE.DotNet.ReadyToRun;
using Epic.OnlineServices.Presence;
using FungleAPI.Utilities.Assets;
using FungleAPI.MonoBehaviours;
using FungleAPI.Networking;
using FungleAPI.Networking.RPCs;
using FungleAPI.Role;
using FungleAPI.Role.Teams;
using FungleAPI.Roles;
using FungleAPI.Utilities;
using HarmonyLib;
using Hazel;
using Il2CppInterop.Runtime;
using Il2CppInterop.Runtime.InteropTypes.Arrays;
using Il2CppInterop.Runtime.Runtime;
using Il2CppSystem.Net;
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
    }
}
