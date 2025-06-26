using FungleAPI.MonoBehaviours;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FungleAPI.Roles;
using FungleAPI.Role.Teams;

namespace FungleAPI.Patches
{
    [HarmonyPatch(typeof(GameManager))]
    public class GameManagerPatch
    {
        [HarmonyPatch("Awake")]
        [HarmonyPostfix]
        private static void OnAwake(GameManager __instance)
        {
            if (__instance.deadBodyPrefab.GetComponent<CustomDeadBody>() == null)
            {
                CustomDeadBody body = __instance.deadBodyPrefab.gameObject.AddComponent<CustomDeadBody>();
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
            else if (endReason == GameOverReason.CrewmatesByTask)
            {
                __instance.RpcCustomEndGame(ModdedTeam.Crewmates);
                return false;
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
                                ChangeableValue<int> value = new ChangeableValue<int>();
                                value.Value++;
                                pair.Add(team, value);
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
                    __instance.RpcCustomEndGame(pair.Keys.ToArray()[0]);
                }
                else if (pair.Count == 0 && neutralKillers.Count == 1 && neutralKillers.Count >= crewmates)
                {
                    __instance.RpcCustomEndGame(neutralKillers);
                }
                else if (pair.Count == 0 && neutralKillers.Count == 0)
                {
                    __instance.RpcCustomEndGame(ModdedTeam.Crewmates);
                }
            }
        }
    }
}
