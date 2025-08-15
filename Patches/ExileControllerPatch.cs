using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FungleAPI.Role.Teams;
using FungleAPI.Roles;
using FungleAPI.Utilities;
using HarmonyLib;

namespace FungleAPI.Patches
{
    [HarmonyPatch(typeof(ExileController))]
    internal class ExileControllerPatchPatch
    {
        [HarmonyPatch("Begin")]
        [HarmonyPostfix]
        public static void BeginPostfix(ExileController __instance)
        {
            if (__instance.initData.networkedPlayer != null && GameOptionsManager.Instance.currentNormalGameOptions.GetBool(AmongUs.GameOptions.BoolOptionNames.ConfirmImpostor))
            {
                string[] tx = StringNames.ExileTextSP.GetString().Split(' ', StringSplitOptions.RemoveEmptyEntries);
                __instance.completeString = __instance.initData.networkedPlayer.PlayerName + " " + tx[1] + " " + tx[2] + " " + __instance.initData.networkedPlayer.Role.NiceName;
                Dictionary<ModdedTeam, ChangeableValue<int>> teams = new Dictionary<ModdedTeam, ChangeableValue<int>>();
                foreach (NetworkedPlayerInfo player in GameData.Instance.AllPlayers)
                {
                    if (!player.IsDead)
                    {
                        ModdedTeam team = player.Role.GetTeam();
                        if (teams.ContainsKey(team))
                        {
                            teams[team].Value++;
                        }
                        else
                        {
                            teams.Add(team, new ChangeableValue<int>(1));
                        }
                    }
                }
                __instance.ImpostorText.text = "Remain: ";
                int count = 0;
                foreach (KeyValuePair<ModdedTeam, ChangeableValue<int>> pair in teams)
                {
                    __instance.ImpostorText.text += pair.Value.ToString() + " " + (pair.Value.Value <= 1 ? pair.Key.TeamName.GetString() : pair.Key.PluralName.GetString());
                    if (count != teams.Count - 1)
                    {
                        __instance.ImpostorText.text += ",";
                    }
                    count++;
                }
            }
        }
        [HarmonyPatch("ReEnableGameplay")]
        [HarmonyPostfix]
        public static void ReEnableGameplayPostfix()
        {
            foreach (CustomAbilityButton button in CustomAbilityButton.Buttons.Values)
            {
                button.Reset();
            }
        }
    }
}
