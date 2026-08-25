using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using FungleAPI.Teams;
using FungleAPI.Role.Utilities;
using FungleAPI.ModCompatibility.MiraSupport;

namespace FungleAPI.Role.Patches
{
    [HarmonyPatch(typeof(PlayerNameColor), nameof(PlayerNameColor.Get), new Type[] { typeof(RoleBehaviour) })]
    internal static class PlayerNameColorPatch
    {
        public static bool Prefix(RoleBehaviour otherPlayerRole, ref Color __result)
        {
            if (PlayerControl.LocalPlayer == null || PlayerControl.LocalPlayer.Data == null || PlayerControl.LocalPlayer.Data.Role == null || otherPlayerRole == null)
            {
                __result = Color.white;
                return false;
            }

            __result = PlayerControl.LocalPlayer.Data.Role.CanSeeRole(otherPlayerRole) ? otherPlayerRole.NameColor : Color.white;
            return false;
        }
    }
}
