using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AmongUs.GameOptions;
using AsmResolver.PE.DotNet.ReadyToRun;
using BepInEx.Unity.IL2CPP.Utils.Collections;
using Epic.OnlineServices.Presence;
using FungleAPI.Components;
using FungleAPI.Event;
using FungleAPI.Event.Types;
using FungleAPI.Networking;
using FungleAPI.Networking.RPCs;
using FungleAPI.Patches;
using FungleAPI.PluginLoading;
using FungleAPI.Role;
using FungleAPI.Role.Patches;
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
using UnityEngine;
using xCloud;
using static UnityEngine.GraphicsBuffer;

namespace FungleAPI.Player
{
    [HarmonyPatch(typeof(PlayerControl))]
    internal static class PlayerControlPatch
    {
        internal static List<Il2CppSystem.Type> AllPlayerComponents = new List<Il2CppSystem.Type>();
        internal static Dictionary<uint, int> CachedColors = new Dictionary<uint, int>();
        [HarmonyPatch("Start")]
        [HarmonyPostfix]
        public static void StartPostfix(PlayerControl __instance)
        {
            if (__instance.GetComponent<PlayerHelper>() == null)
            {
                DoStart(__instance);
            }
        }
        [HarmonyPatch("SetKillTimer")]
        [HarmonyPrefix]
        public static bool SetKillTimerPrefix(PlayerControl __instance, [HarmonyArgument(0)] float time)
        {
            if (__instance.Data.Role.CanUseKillButton)
            {
                float @float = RoleConfigManager.KillConfig.Cooldown();
                if (@float <= 0f)
                {
                    return false;
                }
                __instance.killTimer = Mathf.Clamp(time, 0f, @float);
                DestroyableSingleton<HudManager>.Instance.KillButton.SetCoolDown(__instance.killTimer, @float);
            }
            return false;
        }
        [HarmonyPatch("RpcMurderPlayer")]
        [HarmonyPrefix]
        public static bool RpcMurderPlayerPrefix(PlayerControl __instance, [HarmonyArgument(0)] PlayerControl target, [HarmonyArgument(1)] bool didSucceed)
        {
            __instance.RpcCustomMurderPlayer(target, MurderResultFlags.DecisionByHost | (didSucceed ? MurderResultFlags.Succeeded : MurderResultFlags.FailedError));
            return false;
        }
        [HarmonyPatch("ClientInitialize")]
        [HarmonyPostfix]
        public static void ClientInitializePostfix(PlayerControl __instance)
        {
            __instance.StartCoroutine(ClientInitialize(__instance).WrapToIl2Cpp());
        }
        [HarmonyPatch("MurderPlayer")]
        [HarmonyPrefix]
        public static bool MurderPlayerPrefix(PlayerControl __instance, [HarmonyArgument(0)] PlayerControl target, [HarmonyArgument(1)] MurderResultFlags resultFlags)
        {
            __instance.CustomMurderPlayer(target, resultFlags, true, true, true, true, true);
            return false;
        }
        [HarmonyPatch("CompleteTask")]
        [HarmonyPostfix]
        public static void CompleteTaskPostfix(PlayerControl __instance, uint idx)
        {
            PlayerTask task = __instance.myTasks.ToArray().First(t => t.Id == idx);
            if (task != null)
            {
                EventManager.CallEvent(new OnCompleteTask() { Player = __instance, Task = task });
            }
        }
        [HarmonyPatch("Die")]
        [HarmonyPostfix]
        public static void DiePostfix(PlayerControl __instance, DeathReason reason)
        {
            EventManager.CallEvent(new OnPlayerDie() { Player = __instance, Reason = reason });
        }
        [HarmonyPatch("ReportDeadBody")]
        [HarmonyPostfix]
        public static void ReportDeadBodyPostfix(PlayerControl __instance, [HarmonyArgument(0)] NetworkedPlayerInfo target)
        {
            EventManager.CallEvent(new OnReportBody() { Reporter = __instance, Target = target, Body = target != null ? Helpers.GetBodyById(target.PlayerId) : null });
        }
        [HarmonyPatch("AdjustLighting")]
        [HarmonyPrefix]
        public static bool AdjustLightingPrefix(PlayerControl __instance)
        {
            RoleConfigManager.LightConfig?.AdjustLighting(__instance);
            return false;
        }
        [HarmonyPatch("IsFlashlightEnabled")]
        [HarmonyPrefix]
        public static bool IsFlashlightEnabledPrefix(PlayerControl __instance, ref bool __result)
        {
            __result = RoleConfigManager.LightConfig.IsFlashlightEnabled(__instance);
            return false;
        }
        public static System.Collections.IEnumerator ClientInitialize(PlayerControl playerControl)
        {
            while (!(GameData.Instance != null && playerControl.Data != null && !playerControl.Data.IsIncomplete))
            {
                yield return null;
            }
            if (CachedColors.TryGetValue(playerControl.Data.NetId, out int color))
            {
                if (playerControl.Data.DefaultOutfit.ColorId == 255)
                {
                    playerControl.SetColor(color);
                    FungleAPIPlugin.Instance.Log.LogWarning("Fixed color for player " + playerControl.Data.PlayerName + " original color: " + color);
                }
                CachedColors.Remove(playerControl.Data.NetId);
            }
        }
        public static void DoStart(PlayerControl player)
        {
            foreach (Il2CppSystem.Type type in AllPlayerComponents)
            {
                player.gameObject.AddComponent(type).SafeCast<PlayerComponent>().player = player;
            }
        }
    }
}
