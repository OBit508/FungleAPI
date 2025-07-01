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
        }
        [HarmonyPrefix]
        [HarmonyPatch("RpcMurderPlayer")]
        public static void PlayerControlMurderPrefix(PlayerControl __instance, [HarmonyArgument(0)] PlayerControl target, [HarmonyArgument(1)] bool didSucceed)
        {
            if (didSucceed && __instance.Data.Role as ICustomRole != null)
            {
                (__instance.Data.Role as ICustomRole).Role.MurderPlayer(target, target.GetBody());
            }
        }
        [HarmonyPatch("FixedUpdate")]
        [HarmonyPostfix]
        public static void OnUpdate(PlayerControl __instance)
        {
            try
            {
                if ((AmongUsClient.Instance.IsGameStarted || AmongUsClient.Instance.NetworkMode == NetworkModes.FreePlay) && __instance.Data.Role != null)
                {
                    foreach (PlayerTask text in PlayerControl.LocalPlayer.myTasks)
                    {
                        if (text.gameObject.GetComponent<ImportantTextTask>() != null)
                        {
                            PlayerControl.LocalPlayer.myTasks.Remove(text);
                        }
                    }
                }
            }
            catch
            {

            }
        }
        public static PlayerAnimator CustomAnimator(this PlayerControl player)
        {
            return player.cosmetics.currentBodySprite.BodySprite.GetComponent<PlayerAnimator>();
        }
    }
}
