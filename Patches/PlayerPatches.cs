using HarmonyLib;
using Hazel;
using Il2CppSystem.Net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Il2CppInterop.Runtime.InteropTypes.Arrays;
using FungleAPI.MonoBehaviours;
using AmongUs.GameOptions;
using Epic.OnlineServices.Presence;
using xCloud;
using UnityEngine;
using FungleAPI.Rpc;
using FungleAPI.Role.Teams;
using FungleAPI.Roles;
using FungleAPI.Role;
using AsmResolver.PE.DotNet.ReadyToRun;

namespace FungleAPI.Patches
{
    [HarmonyPatch(typeof(PlayerControl))]
    public static class PlayerPatches
    {
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
    }
}
