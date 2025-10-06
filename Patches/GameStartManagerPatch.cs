using FungleAPI.Components;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FungleAPI.Patches
{
    [HarmonyPatch(typeof(GameStartManager), "Update")]
    internal static class GameStartManagerPatch
    {
        public static bool UsingCustomText;
        public static void Postfix(GameStartManager __instance)
        {
            __instance.StartButton.SetButtonEnableState(__instance.LastPlayerCount >= __instance.MinPlayers && LobbyWarningText.nonModdedPlayers.Count <= 0);
            ActionMapGlyphDisplay startButtonGlyph = __instance.StartButtonGlyph;
            if (startButtonGlyph != null)
            {
                startButtonGlyph.SetColor((__instance.LastPlayerCount >= __instance.MinPlayers && LobbyWarningText.nonModdedPlayers.Count <= 0) ? Palette.EnabledColor : Palette.DisabledClear);
            }
            if (LobbyWarningText.nonModdedPlayers.Count > 0)
            {
                __instance.StartButton.ChangeButtonText("The game cannot start because certain players do not have the FungleAPI installed.");
                UsingCustomText = true;
            }
            else if (UsingCustomText)
            {
                if (__instance.LastPlayerCount >= __instance.MinPlayers)
                {
                    __instance.StartButton.ChangeButtonText(DestroyableSingleton<TranslationController>.Instance.GetString(StringNames.StartLabel));
                    __instance.GameStartTextClient.text = DestroyableSingleton<TranslationController>.Instance.GetString(StringNames.WaitingForHost);
                }
                else
                {
                    __instance.StartButton.ChangeButtonText(DestroyableSingleton<TranslationController>.Instance.GetString(StringNames.WaitingForPlayers));
                    __instance.GameStartTextClient.text = DestroyableSingleton<TranslationController>.Instance.GetString(StringNames.WaitingForPlayers);
                }
                UsingCustomText = false;
            }
        }
    }
}
