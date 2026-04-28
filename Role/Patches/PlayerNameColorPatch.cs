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
    [HarmonyPatch(typeof(PlayerNameColor), "Get", new Type[] { typeof(RoleBehaviour) })]
    internal static class PlayerNameColorPatch
    {
        public static bool Prefix([HarmonyArgument(0)] RoleBehaviour otherPlayerRole, ref Color __result)
        {
            RoleBehaviour role = PlayerControl.LocalPlayer.Data.Role;
            if (role.GetTeam() != ModdedTeamManager.Crewmates && role.GetTeam() != ModdedTeamManager.Impostors && role.GetTeam() == otherPlayerRole.GetTeam() && role.GetTeam().KnowMembers)
            {
                if (otherPlayerRole.CustomRole() != null)
                {
                    __result = otherPlayerRole.CustomRole().Configuration.ShowedTeamColor;
                    return false;
                }
                __result = otherPlayerRole.NameColor;
                return false;
            }
            __result = Color.white;
            return false;
        }
    }
}
