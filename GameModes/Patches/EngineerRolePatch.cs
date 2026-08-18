using AmongUs.Data;
using FungleAPI.Api;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FungleAPI.GameModes.Patches
{
    [HarmonyPatch(typeof(EngineerRole))]
    internal static class EngineerRolePatch
    {
        [HarmonyPatch(nameof(EngineerRole.Initialize))]
        [HarmonyPostfix]
        public static void InitializePostfix(EngineerRole __instance)
        {
            if (GameModeManager.GetCurrentGameMode() is HideNSeekMode hideNSeekMode)
            {
                __instance.usesRemaining = hideNSeekMode.GetCrewmateVentUses();
                DestroyableSingleton<HudManager>.Instance.AbilityButton.OverrideText(DestroyableSingleton<TranslationController>.Instance.GetString(StringNames.HideActionButton));
                DestroyableSingleton<HudManager>.Instance.AbilityButton.SetUsesRemaining(__instance.usesRemaining);
            }
        }
        [HarmonyPatch(nameof(EngineerRole.UseAbility))]
        [HarmonyPostfix]
        public static void UseAbilityPostfix(EngineerRole __instance)
        {
            if (GameModeManager.GetCurrentGameMode() is HideNSeekMode hideNSeekMode)
            {
                PlayerControl localPlayer = PlayerControl.LocalPlayer;
                if (!__instance.currentTarget)
                {
                    return;
                }
                bool flag;
                bool flag2;
                __instance.currentTarget.CanUse(localPlayer.Data, out flag, out flag2);
                if (__instance.isActiveAndEnabled && !__instance.IsCoolingDown && flag)
                {
                    __instance.inVentTimeRemaining = __instance.GetVentTime();
                    bool flag3 = localPlayer.inVent && !localPlayer.walkingToVent;
                    __instance.currentTarget.Use();
                    __instance.usesRemaining--;
                    DestroyableSingleton<HudManager>.Instance.AbilityButton.SetUsesRemaining(__instance.usesRemaining);
                    DataManager.Player.Stats.IncrementStat(StatID.HideAndSeek_TimesVented);
                }
            }
        }
    }
}
