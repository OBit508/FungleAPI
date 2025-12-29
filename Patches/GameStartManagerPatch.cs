using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FungleAPI.Components;
using FungleAPI.Translation;
using FungleAPI.Utilities;
using HarmonyLib;
using UnityEngine;
using UnityEngine.Events;

namespace FungleAPI.Patches
{
    [HarmonyPatch(typeof(GameStartManager))]
    internal static class GameStartManagerPatch
    {
        [HarmonyPatch("Start")]
        [HarmonyPostfix]
        public static void StartPostfix(GameStartManager __instance)
        {
            if (AmongUsClient.Instance.NetworkMode != NetworkModes.OnlineGame)
            {
                return;
            }
            __instance.HostPublicButton.enabled = false;
            __instance.HostPrivateButton.transform.FindChild("Inactive").GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f, 0.5f);
            __instance.HostPrivateButton.SetNewAction(delegate
            {
                Helpers.ShowPopup(FungleTranslation.ChangeToPublicText.GetString());
            });
            if (AmongUsClient.Instance.AmHost && AmongUsClient.Instance.IsGamePublic)
            {
                AmongUsClient.Instance.ChangeGamePublic(false);
            }
        }
    }
}
