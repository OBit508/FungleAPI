using AmongUs.GameOptions;
using AsmResolver.PE.DotNet.ReadyToRun;
using Epic.OnlineServices.Presence;
using FungleAPI.MonoBehaviours;
using FungleAPI.Role;
using FungleAPI.Role.Teams;
using FungleAPI.Roles;
using FungleAPI.Rpc;
using HarmonyLib;
using Hazel;
using Il2CppInterop.Runtime.InteropTypes.Arrays;
using Il2CppSystem.Net;
using Rewired;
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
    public static class PlayerPatches
    {
        internal static List<Il2CppSystem.Type> AllPlayerComponents = new List<Il2CppSystem.Type>();
        [HarmonyPatch("Start")]
        [HarmonyPostfix]
        public static void OnStart(PlayerControl __instance)
        {
            foreach (PlayerBodySprite body in __instance.cosmetics.bodySprites)
            {
                PlayerAnimator animator = body.BodySprite.gameObject.AddComponent<PlayerAnimator>();
                animator.Player = __instance;
                animator.Animator = SpriteAnimator.AddCustomAnimator(body.BodySprite);
            }
            __instance.gameObject.AddComponent<PlayerHelper>();
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
            if (__instance.Data.Role.CustomRole() != null && active)
            {
                __instance.cosmetics.currentBodySprite.BodySprite.material.SetFloat("_Outline", 1f);
                __instance.cosmetics.currentBodySprite.BodySprite.material.SetColor("_OutlineColor", __instance.Data.Role.CustomRole().CachedConfiguration.OutlineColor);
                return false;
            }
            return true;
        }
        public static PlayerAnimator CustomAnimator(this PlayerControl player)
        {
            return player.cosmetics.currentBodySprite.BodySprite.GetComponent<PlayerAnimator>();
        }
        public static void RpcCustomMurderPlayer(this PlayerControl killer, PlayerControl target, MurderResultFlags resultFlags, bool resetKillTimer = true, bool createDeadBody = true, bool teleportMurderer = true, bool showKillAnim = true, bool playKillSound = true)
        {
            CustomRpcManager.GetInstance<RpcCustomMurder>().Send((killer, target, resultFlags, resetKillTimer, createDeadBody, teleportMurderer, showKillAnim, playKillSound), killer.NetId);
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
