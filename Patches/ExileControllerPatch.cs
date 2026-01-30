using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FungleAPI.Role;
using FungleAPI.Utilities;
using HarmonyLib;
using UnityEngine.UIElements;
using FungleAPI.Translation;
using FungleAPI.Hud;
using FungleAPI.Teams;
using FungleAPI.Event;
using FungleAPI.Event.Types;

namespace FungleAPI.Patches
{
    [HarmonyPatch(typeof(ExileController))]
    internal static class ExileControllerPatch
    {
        [HarmonyPatch("Begin")]
        [HarmonyPostfix]
        public static void BeginPostfix(ExileController __instance)
        {
            if (__instance.initData.networkedPlayer != null && __instance.initData.networkedPlayer.Role != null && __instance.initData.networkedPlayer.Role.CustomRole() != null && GameOptionsManager.Instance.currentNormalGameOptions.GetBool(AmongUs.GameOptions.BoolOptionNames.ConfirmImpostor))
            {
                __instance.completeString = __instance.initData.networkedPlayer.Role.CustomRole().ExileText(__instance);
            }
            __instance.ImpostorText.text = FungleTranslation.TeamsRemainText.GetString();
            Dictionary<ModdedTeam, ChangeableValue<int>> teams = new Dictionary<ModdedTeam, ChangeableValue<int>>();
            foreach (PlayerControl player in PlayerControl.AllPlayerControls)
            {
                if (player.Data != __instance.initData.networkedPlayer && !player.Data.IsDead)
                {
                    ModdedTeam team = player.Data.Role.GetTeam();
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
            foreach (KeyValuePair<ModdedTeam, ChangeableValue<int>> pair in teams)
            {
                __instance.ImpostorText.text += pair.Value.Value.ToString() + " " + pair.Key.TeamColor.ToTextColor() + (pair.Value.Value == 1 ? pair.Key.TeamName.GetString() : pair.Key.PluralName.GetString()) + "</color>" + (pair.Key == teams.Last().Key ? "" : ", ");
            }
            EventManager.CallEvent(new OnEject() { Controller = __instance, Target = __instance.initData.networkedPlayer });
        }
        [HarmonyPatch("ReEnableGameplay")]
        [HarmonyPostfix]
        public static void ReEnableGameplayPostfix()
        {
            foreach (CustomAbilityButton button in CustomAbilityButton.Buttons.Values)
            {
                button.Reset(CustomAbilityButton.ResetType.EndMeeting);
            }
        }
    }
}
