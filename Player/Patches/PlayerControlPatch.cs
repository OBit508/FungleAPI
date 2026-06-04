using FungleAPI.Components;
using FungleAPI.Event;
using FungleAPI.Event.Vanilla;
using FungleAPI.Event.Vanilla.Player;
using FungleAPI.Role;
using FungleAPI.Utilities;
using HarmonyLib;
using System.Collections.Generic;
using UnityEngine;

namespace FungleAPI.Player.Patches
{
    [HarmonyPatch(typeof(PlayerControl))]
    internal static class PlayerControlPatch
    {
        internal static List<Il2CppSystem.Type> AllPlayerComponents = new List<Il2CppSystem.Type>();
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
        public static bool SetKillTimerPrefix(PlayerControl __instance, float time)
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
        public static bool RpcMurderPlayerPrefix(PlayerControl __instance, PlayerControl target, bool didSucceed)
        {
            return !EventManager.CallEvent(new BeforeMurderEvent(target, (didSucceed ? MurderResultFlags.Succeeded : MurderResultFlags.FailedError) | MurderResultFlags.DecisionByHost)).Cancelled;
        }
        [HarmonyPatch("CheckMurder")]
        [HarmonyPrefix]
        public static bool CheckMurderPrefix(PlayerControl __instance, PlayerControl target)
        {
            __instance.CheckCustomMurder(target);
            return false;
        }
        [HarmonyPatch("MurderPlayer")]
        [HarmonyPrefix]
        public static bool MurderPlayerPrefix(PlayerControl __instance, PlayerControl target, MurderResultFlags resultFlags)
        {
            __instance.CustomMurderPlayer(target, resultFlags);
            return false;
        }
        [HarmonyPatch("CompleteTask")]
        [HarmonyPostfix]
        public static void CompleteTaskPostfix(PlayerControl __instance, uint idx)
        {
            EventManager.CallEvent(new CompleteTaskEvent(__instance, idx));
        }
        [HarmonyPatch("Die")]
        [HarmonyPostfix]
        public static void DiePostfix(PlayerControl __instance, DeathReason reason)
        {
            EventManager.CallEvent(new PlayerDieEvent(__instance, reason));
        }
        [HarmonyPatch("ReportDeadBody")]
        [HarmonyPostfix]
        public static void ReportDeadBodyPostfix(PlayerControl __instance, NetworkedPlayerInfo target)
        {
            EventManager.CallEvent(new ReportBodyEvent(__instance, target));
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
        public static void DoStart(PlayerControl player)
        {
            foreach (Il2CppSystem.Type type in AllPlayerComponents)
            {
                player.gameObject.AddComponent(type).SafeCast<PlayerComponent>().player = player;
            }
        }
    }
}
