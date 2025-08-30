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
        public static void OnStart(PlayerControl __instance)
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
            RpcCustomMurderPlayer(__instance, target, MurderResultFlags.Succeeded);
            return false;
        }
        [HarmonyPatch("ToggleHighlight")]
        [HarmonyPrefix]
        public static bool OnToggleHighlight(PlayerControl __instance, [HarmonyArgument(0)] bool active)
        {
            __instance.cosmetics.SetOutline(active, new Il2CppSystem.Nullable<Color>(__instance.Data.Role.CustomRole().Configuration.OutlineColor));
            return false;
        }
        public static void RpcCustomMurderPlayer(this PlayerControl killer, PlayerControl target, MurderResultFlags resultFlags, bool resetKillTimer = true, bool createDeadBody = true, bool teleportMurderer = true, bool showKillAnim = true, bool playKillSound = true)
        {
            CustomRpcManager.Instance<RpcCustomMurder>().Send((killer, target, resultFlags, resetKillTimer, createDeadBody, teleportMurderer, showKillAnim, playKillSound), killer.NetId);
        }
        public static T GetPlayerComponent<T>(this PlayerControl player) where T : PlayerComponent
        {
            return player.GetComponent<T>();
        }
        public static PlayerControl GetClosest(this PlayerControl target)
        {
            PlayerControl closest = null;
            float dis = target.Data.Role.GetAbilityDistance();
            foreach (PlayerControl player in PlayerControl.AllPlayerControls)
            {
                Vector3 center = target.Collider.bounds.center;
                Vector3 position = player.transform.position;
                float num = Vector2.Distance(center, position);
                if (player != target && !player.Data.IsDead && !PhysicsHelpers.AnythingBetween(target.Collider, center, position, Constants.ShipOnlyMask, false) && num < dis)
                {
                    closest = player;
                    dis = num;
                }
            }
            return closest;
        }
    }
    
}
