using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FungleAPI.Role.Teams;
using FungleAPI.Role;
using FungleAPI.Utilities;
using HarmonyLib;
using UnityEngine.UIElements;
using FungleAPI.Translation;

namespace FungleAPI.Patches
{
    [HarmonyPatch(typeof(ExileController))]
    internal static class ExileControllerPatch
    {
        public static Translator remainText;
        public static StringNames TeamsRemainText
        {
            get
            {
                if (remainText == null)
                {
                    remainText = new Translator("Remaining Teams: ");
                    remainText.AddTranslation(SupportedLangs.Latam, "Equipos restantes: ");
                    remainText.AddTranslation(SupportedLangs.Brazilian, "Times Restantes: ");
                    remainText.AddTranslation(SupportedLangs.Portuguese, "Times Restantes: ");
                    remainText.AddTranslation(SupportedLangs.Korean, "남은 팀: ");
                    remainText.AddTranslation(SupportedLangs.Russian, "Оставшиеся команды: ");
                    remainText.AddTranslation(SupportedLangs.Dutch, "Resterende teams: ");
                    remainText.AddTranslation(SupportedLangs.Filipino, "Natitirang mga koponan: ");
                    remainText.AddTranslation(SupportedLangs.French, "Équipes restantes : ");
                    remainText.AddTranslation(SupportedLangs.German, "Verbleibende Teams: ");
                    remainText.AddTranslation(SupportedLangs.Italian, "Squadre rimanenti: ");
                    remainText.AddTranslation(SupportedLangs.Japanese, "残りのチーム: ");
                    remainText.AddTranslation(SupportedLangs.Spanish, "Equipos restantes: ");
                    remainText.AddTranslation(SupportedLangs.SChinese, "剩余队伍: ");
                    remainText.AddTranslation(SupportedLangs.TChinese, "剩餘隊伍: ");
                    remainText.AddTranslation(SupportedLangs.Irish, "Foirne atá fágtha: ");
                }
                return remainText.StringName;
            }
        }
        [HarmonyPatch("Begin")]
        [HarmonyPostfix]
        public static void BeginPostfix(ExileController __instance)
        {
            if (__instance.initData.networkedPlayer != null && __instance.initData.networkedPlayer.Role != null && __instance.initData.networkedPlayer.Role.CustomRole() != null && GameOptionsManager.Instance.currentNormalGameOptions.GetBool(AmongUs.GameOptions.BoolOptionNames.ConfirmImpostor))
            {
                __instance.completeString = __instance.initData.networkedPlayer.Role.CustomRole().ExileText(__instance);
            }
            __instance.ImpostorText.text = TeamsRemainText.GetString();
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
