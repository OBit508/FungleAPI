using AmongUs.GameOptions;
using FungleAPI.MonoBehaviours;
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
            if (__instance.deadBodyPrefab != null)
            {
                GameObject bodyPrefab = __instance.deadBodyPrefab.gameObject;
                SpriteRenderer[] renderers = __instance.deadBodyPrefab.bodyRenderers;
                SpriteRenderer renderer = __instance.deadBodyPrefab.bloodSplatter;
                GameObject.Destroy(__instance.deadBodyPrefab);
                ModdedDeadBody body = bodyPrefab.AddComponent<ModdedDeadBody>();
                body.bodyRenderers = renderers;
                body.bloodSplatter = renderer;
                body.myCollider = body.GetComponent<Collider2D>();
                __instance.deadBodyPrefab = body;
            }
        }
        [HarmonyPatch("CheckEndGameViaTasks")]
        [HarmonyPrefix]
        private static bool CheckEndGameViaTasksPrefix(GameManager __instance, ref bool __result)
        {
            GameData.Instance.RecomputeTaskCounts();
            bool flag = true;
            foreach (PlayerControl player in PlayerControl.AllPlayerControls)
            {
                if (player.Data.Role.CanDoTasks())
                {
                    foreach (PlayerTask task in player.myTasks)
                    {
                        if (task.SafeCast<NormalPlayerTask>() != null && !task.SafeCast<NormalPlayerTask>().IsComplete)
                        {
                            flag = false;
                        }
                    }
                }
            }
            if (flag)
            {
                RpcCustomEndGame(__instance, ModdedTeam.Crewmates);
                __result = true;
                return false;
            }
            __result = false;
            return false;
        }
        public static void RpcCustomEndGame(this GameManager manager, ModdedTeam team)
        {
            manager.RpcEndGame(team.WinReason, false);
        }
    }
}
