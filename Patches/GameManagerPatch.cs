using FungleAPI.MonoBehaviours;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FungleAPI.Roles;

namespace FungleAPI.Patches
{
    [HarmonyPatch(typeof(GameManager))]
    public class GameManagerPatch
    {
        public static bool AutoEndGame = true;
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
            return false;
        }
        internal static bool customEnd = true;
        [HarmonyPatch("FixedUpdate")]
        [HarmonyPrefix]
        private static void OnUpdate(GameManager __instance)
        {
            if (AutoEndGame && AmongUsClient.Instance.IsGameStarted)
            {
                int taskRemain = 0;
                int impostorsRemain = 0;
                int crewmatesRemain = 0;
                foreach (PlayerControl player in PlayerControl.AllPlayerControls)
                {
                    if (player.Data.Role.GetTeam() == ModdedTeam.Crewmates)
                    {
                        if (!player.Data.IsDead)
                        {
                            crewmatesRemain++;
                        }
                        foreach (PlayerTask task in player.myTasks)
                        {
                            if (task.gameObject.GetComponent<NormalPlayerTask>() != null && !task.gameObject.GetComponent<NormalPlayerTask>().IsComplete)
                            {
                                taskRemain++;
                            }
                        }
                    }
                    else if (player.Data.Role.GetTeam() == ModdedTeam.Impostors && !player.Data.IsDead)
                    {
                        impostorsRemain++;
                    }
                }
                if (taskRemain <= 0 || impostorsRemain <= 0)
                {
                    __instance.RpcCustomEndGame(ModdedTeam.Crewmates);
                }
                else if (impostorsRemain >= crewmatesRemain)
                {
                    __instance.RpcCustomEndGame(ModdedTeam.Impostors);
                }
            }
        }
    }
}
