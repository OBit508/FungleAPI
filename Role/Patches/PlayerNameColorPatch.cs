using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using FungleAPI.Teams;
using FungleAPI.Role.Utilities;

namespace FungleAPI.Role.Patches
{
    [HarmonyPatch(typeof(PlayerNameColor), nameof(PlayerNameColor.Get), new Type[] { typeof(RoleBehaviour) })]
    internal static class PlayerNameColorPatch
    {
        public static bool Prefix(RoleBehaviour otherPlayerRole, ref Color __result)
        {
            RoleBehaviour role = PlayerControl.LocalPlayer.Data.Role;
            ModdedTeam team = role.GetTeam();

            if (team == otherPlayerRole.GetTeam() && team.KnowMembers || otherPlayerRole.Player != null && otherPlayerRole.Player.AmOwner)
            {
                __result = otherPlayerRole.NameColor;
                return false;
            }
            __result = Color.white;
            return false;
        }
    }
}
