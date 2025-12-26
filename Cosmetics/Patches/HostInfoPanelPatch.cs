using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;
using UnityEngine;
using static Il2CppSystem.Net.Http.Headers.Parser;

namespace FungleAPI.Cosmetics.Patches
{
    [HarmonyPatch(typeof(HostInfoPanel), "SetUp")]
    internal static class HostInfoPanelPatch
    {
        public static void Postfix(HostInfoPanel __instance)
        {
            NetworkedPlayerInfo host = GameData.Instance.GetHost();
            string text = ColorUtility.ToHtmlStringRGB(CosmeticManager.GetBaseColor(__instance.player.ColorId));
            if (host == null || host.IsIncomplete)
            {
                return;
            }
            if (AmongUsClient.Instance.AmHost)
            {
                __instance.playerName.text = (string.IsNullOrEmpty(host.PlayerName) ? "..." : string.Concat(new string[]
                {
                "<color=#",
                text,
                ">",
                host.PlayerName,
                "</color>"
                })) + "  <size=90%><b><font=\"Barlow-BoldItalic SDF\" material=\"Barlow-BoldItalic SDF Outline\">" + DestroyableSingleton<TranslationController>.Instance.GetString(StringNames.HostYouLabel);
            }
            else
            {
                __instance.playerName.text = (string.IsNullOrEmpty(host.PlayerName) ? "..." : string.Concat(new string[]
                {
                "<color=#",
                text,
                ">",
                host.PlayerName,
                "</color>"
                })) + " (" + __instance.player.ColorBlindName + ")";
            }
        }
    }
}
