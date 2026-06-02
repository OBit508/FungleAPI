using FungleAPI.Components;
using HarmonyLib;
using Il2CppInterop.Runtime.InteropTypes.Arrays;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace FungleAPI.Role.Patches
{
    [HarmonyPatch(typeof(PlayerVoteArea), nameof(PlayerVoteArea.SetCosmetics))]
    internal static class MeetingIntroAnimationPatch
    {
        public static void Postfix(PlayerVoteArea __instance, NetworkedPlayerInfo playerInfo)
        {
            PlayerVoteAreaHelper playerVoteAreaHelper = __instance.gameObject.AddComponent<PlayerVoteAreaHelper>();
            playerVoteAreaHelper.VoteArea = __instance;
            playerVoteAreaHelper.Owner = playerInfo;
        }
    }
}
