using FungleAPI.Components;
using FungleAPI.Event;
using FungleAPI.Event.Vanilla;
using FungleAPI.GameModes;
using FungleAPI.Hud;
using FungleAPI.Player;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace FungleAPI.GlobalPatches
{
    [HarmonyPatch(typeof(MeetingHud))]
    internal static class MeetingPatch
    {
        [HarmonyPatch("Start")]
        [HarmonyPostfix]
        public static void StartPostfix(MeetingHud __instance)
        {
            foreach (CustomAbilityButton button in HudHelper.Buttons.Values)
            {
                if (button.Button != null)
                {
                    button.MeetingStart(__instance);
                }
            }
            EventManager.CallEvent(new StartMeetingEvent(__instance));
        }
        [HarmonyPostfix]
        [HarmonyPatch("Close")]
        public static void ClosePostfix(MeetingHud __instance)
        {
            if (GameModeManager.GetCurrentGameMode().GetChatInGame())
            {
                HudManager.Instance.Chat.SetVisible(true);
            }
        }
        [HarmonyPostfix]
        [HarmonyPatch("OnDestroy")]
        public static void OnDestroyPostfix(MeetingHud __instance)
        {
            EventManager.CallEvent(new EndMeetingEvent(__instance));
        }
    }
}
