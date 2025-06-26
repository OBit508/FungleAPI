using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FungleAPI.Roles;
using HarmonyLib;
using UnityEngine;

namespace FungleAPI.Patches
{
    [HarmonyPatch(typeof(MeetingHud))]
    internal class MeetingPatch
    {
        [HarmonyPatch("Update")]
        [HarmonyPrefix]
        public static void OnUpdate(MeetingHud __instance)
        {
            PlayerControl.LocalPlayer.GetVoteArea().NameText.color = PlayerControl.LocalPlayer.Data.Role.NameColor;
        }
    }
}
