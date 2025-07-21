using AmongUs.GameOptions;
using FungleAPI.MonoBehaviours;
using FungleAPI.Role.Teams;
using FungleAPI.Roles;
using FungleAPI.Rpc;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace FungleAPI.Patches
{
    [HarmonyPatch(typeof(GameManager))]
    public static class GameManagerPatch
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
                CustomDeadBody body = bodyPrefab.AddComponent<CustomDeadBody>();
                body.bodyRenderers = renderers;
                body.bloodSplatter = renderer;
                body.myCollider = body.GetComponent<Collider2D>();
                __instance.deadBodyPrefab = body;
            }
        }
        [HarmonyPatch("RpcEndGame")]
        [HarmonyPrefix]
        private static bool OnRpcEndGame(GameManager __instance, [HarmonyArgument(0)] GameOverReason endReason)
        {
            if (customEnd)
            {
                customEnd = false;
                return true;
            }
            if (endReason == GameOverReason.CrewmatesByTask)
            {
                customEnd = true;
                RpcCustomEndGame(__instance, ModdedTeam.Crewmates);
            }
            return false;
        }
        internal static bool customEnd = true;
        [HarmonyPatch("FixedUpdate")]
        [HarmonyPrefix]
        private static void OnUpdate(GameManager __instance)
        {
            if (AmongUsClient.Instance.IsGameStarted)
            {
                Dictionary<ModdedTeam, ChangeableValue<int>> pair = new Dictionary<ModdedTeam, ChangeableValue<int>>();
                int crewmates = 0;
                List<PlayerControl> neutralKillers = new List<PlayerControl>();
                foreach (PlayerControl player in PlayerControl.AllPlayerControls)
                {
                    if (!player.Data.IsDead)
                    {
                        ModdedTeam team = player.Data.Role.GetTeam();
                        if (team == ModdedTeam.Crewmates)
                        {
                            crewmates++;
                        }
                        else if (team == ModdedTeam.Neutrals && player.Data.Role.CanKill())
                        {
                            neutralKillers.Add(player);
                        }
                        else
                        {
                            if (!pair.Keys.Contains(team))
                            {
                                pair.Add(team, new ChangeableValue<int>(1));
                            }
                            else
                            {
                                foreach (KeyValuePair<ModdedTeam, ChangeableValue<int>> pair2 in pair)
                                {
                                    if (pair2.Key == team)
                                    {
                                        pair2.Value.Value++;
                                    }
                                }
                            }
                        }
                    }
                }
                if (pair.Count == 1 && pair.Values.ToArray()[0].Value >= crewmates && neutralKillers.Count == 0)
                {
                    RpcCustomEndGame(__instance, pair.Keys.ToArray()[0]);
                }
                else if (pair.Count == 0 && neutralKillers.Count == 1 && neutralKillers.Count >= crewmates)
                {
                    RpcCustomEndGame(__instance, ModdedTeam.Neutrals);
                }
                else if (pair.Count == 0 && neutralKillers.Count == 0)
                {
                    RpcCustomEndGame(__instance, ModdedTeam.Crewmates);
                }
            }
        }
        public static void RpcCustomEndGame(this GameManager manager, ModdedTeam team)
        {
            customEnd = true;
            manager.RpcEndGame(team.WinReason, false);
        }
    }
}
