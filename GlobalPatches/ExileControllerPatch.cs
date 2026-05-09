using FungleAPI.Event;
using FungleAPI.Event.Vanilla;
using FungleAPI.Hud;
using FungleAPI.Role;
using FungleAPI.Role.Utilities;
using FungleAPI.Teams;
using FungleAPI.Translation;
using FungleAPI.Utilities;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.UIElements;

namespace FungleAPI.Patches
{
    [HarmonyPatch(typeof(ExileController))]
    internal static class ExileControllerPatch
    {
        [HarmonyPatch("Begin")]
        [HarmonyPrefix]
        public static bool BeginPrefix(ExileController __instance, ExileController.InitProperties init)
        {
            if (EventManager.CallEvent(new BeforeEjectionEvent(__instance, init)).Cancelled)
            {
                return false;
            }
            if (__instance.specialInputHandler != null)
            {
                __instance.specialInputHandler.disableVirtualCursor = true;
            }
            ExileController.Instance = __instance;
            ControllerManager.Instance.CloseAndResetAll();
            __instance.initData = init;
            __instance.Text.gameObject.SetActive(false);
            __instance.Text.text = string.Empty;
            if (DestroyableSingleton<HudManager>.InstanceExists)
            {
                DestroyableSingleton<HudManager>.Instance.SetMapButtonEnabled(false);
            }
            if (init != null && init.outfit != null)
            {
                if (!init.confirmImpostor)
                {
                    __instance.completeString = DestroyableSingleton<TranslationController>.Instance.GetString(StringNames.ExileTextNonConfirm, new Il2CppSystem.Object[]
                    {
                    init.outfit.PlayerName
                    });
                }
                else if (__instance.initData.networkedPlayer != null && __instance.initData.networkedPlayer.Role != null)
                {
                    ICustomRole customRole = __instance.initData.networkedPlayer.Role.CustomRole();
                    if (customRole != null)
                    {
                        __instance.completeString = customRole.ExileText(__instance);
                    }
                    else
                    {
                        string[] tx = StringNames.ExileTextSP.GetString().Split(' ', StringSplitOptions.RemoveEmptyEntries);
                        __instance.completeString = __instance.initData.networkedPlayer.PlayerName + " " + tx[1] + " " + tx[2] + " " + __instance.initData.networkedPlayer.Role.NiceName;
                    }
                }
                __instance.Player.UpdateFromPlayerOutfit(init.outfit, PlayerMaterial.MaskType.Exile, false, false, new Action(delegate
                {
                    SkinViewData skinViewData;
                    if (GameManager.Instance != null)
                    {
                        skinViewData = ShipStatus.Instance.CosmeticsCache.GetSkin(__instance.initData.outfit.SkinId);
                    }
                    else
                    {
                        skinViewData = __instance.Player.GetSkinView();
                    }
                    if (GameManager.Instance != null && !DestroyableSingleton<HatManager>.Instance.CheckLongModeValidCosmetic(init.outfit.SkinId, __instance.Player.GetIgnoreLongMode()))
                    {
                        skinViewData = ShipStatus.Instance.CosmeticsCache.GetSkin("skin_None");
                    }
                    if (__instance.useIdleAnim)
                    {
                        __instance.Player.FixSkinSprite(skinViewData.IdleFrame);
                        return;
                    }
                    __instance.Player.FixSkinSprite(skinViewData.EjectFrame);
                }), false);
                __instance.Player.ToggleName(false);
                if (!__instance.useIdleAnim)
                {
                    __instance.Player.SetCustomHatPosition(__instance.exileHatPosition);
                    __instance.Player.SetCustomVisorPosition(__instance.exileVisorPosition);
                }
            }
            else
            {
                if (init.voteTie)
                {
                    __instance.completeString = DestroyableSingleton<TranslationController>.Instance.GetString(StringNames.NoExileTie);
                }
                else
                {
                    __instance.completeString = DestroyableSingleton<TranslationController>.Instance.GetString(StringNames.NoExileSkip);
                }
                __instance.Player.gameObject.SetActive(false);
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
            __instance.StartCoroutine(__instance.Animate());
            EventManager.CallEvent(new AfterEjectionEvent(__instance, init));
            return false;
        }
        [HarmonyPatch("ReEnableGameplay")]
        [HarmonyPostfix]
        public static void ReEnableGameplayPostfix()
        {
            foreach (CustomAbilityButton button in HudHelper.Buttons.Values)
            {
                button.Reset(CustomAbilityButton.ResetType.EndMeeting);
            }
        }
    }
}
