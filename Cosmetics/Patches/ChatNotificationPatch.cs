using FungleAPI.Cosmetics.Colors;
using FungleAPI.Extensions;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FungleAPI.Cosmetics.Patches
{
    [HarmonyPatch(typeof(ChatNotification))]
    internal static class ChatNotificationPatch
    {
        public static string playerNameText;
        public static SpecialColor SpcColor;
        [HarmonyPatch(nameof(ChatNotification.Update))]
        [HarmonyPostfix]
        public static void UpdatePostfix(ChatNotification __instance)
        {
            if (SpcColor == null) return;

            __instance.playerNameText.text = SpcColor.BackColor.ToTextColor() + playerNameText;
            __instance.playerNameText.outlineColor = SpcColor.BaseColor.Darken();
        }
        [HarmonyPatch(nameof(ChatNotification.SetUp))]
        [HarmonyPrefix]
        public static void SetUpPrefix(ChatNotification __instance, PlayerControl sender)
        {
            if (CosmeticManager.IsSpecialColor(sender.cosmetics.ColorId, out SpcColor))
            {
                playerNameText = string.IsNullOrEmpty(sender.Data.PlayerName) ? "..." : sender.Data.PlayerName;
            }
        }
    }
}
