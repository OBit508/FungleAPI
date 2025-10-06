using AmongUs.GameOptions;
using BepInEx;
using BepInEx.Unity.IL2CPP;
using FungleAPI.Components;
using FungleAPI.Networking;
using FungleAPI.Networking.RPCs;
using FungleAPI.Patches;
using FungleAPI.Role;
using FungleAPI.Role.Teams;
using HarmonyLib;
using Hazel;
using Il2CppSystem.Text;
using InnerNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using UnityEngine;

namespace FungleAPI.Role.Patches
{
    [HarmonyPatch(typeof(RoleBehaviour))]
    internal static class RoleBehaviourPatch
    {
        [HarmonyPrefix]
        [HarmonyPatch("TeamColor", MethodType.Getter)]
        public static bool TeamColorPrefix(RoleBehaviour __instance, ref Color __result)
        { 
            ICustomRole role = __instance.CustomRole();
            if (role != null)
            {
                if (role.Configuration.ShowTeamColor)
                {
                    __result = role.Team.TeamColor;
                }
                else
                {
                    __result = role.RoleColor;
                }
                return false;
            }
            __result = __instance.GetTeam().TeamColor;
            return false;
        }
    }
}
