using FungleAPI.Role;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using FungleAPI.Teams;

namespace FungleAPI.Role.Patches
{
    [HarmonyPatch(typeof(PlayerNameColor), "Get", new Type[] { typeof(RoleBehaviour) })]
    internal static class PlayerNameColorPatch
    {
        public static bool Prefix([HarmonyArgument(0)] RoleBehaviour otherPlayerRole, ref Color __result)
        {
            RoleBehaviour role = PlayerControl.LocalPlayer.Data.Role;
            if (role.GetTeam() != ModdedTeam.Crewmates && role.GetTeam() != ModdedTeam.Impostors && role.GetTeam() == otherPlayerRole.GetTeam() && role.GetTeam().KnowMembers)
            {
                if (otherPlayerRole.CustomRole() != null)
                {
                    if (otherPlayerRole.CustomRole().ShowTeamColor)
                    {
                        __result = otherPlayerRole.GetTeam().TeamColor;
                    }
                    else
                    {
                        __result = otherPlayerRole.CustomRole().RoleColor;
                    }
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
