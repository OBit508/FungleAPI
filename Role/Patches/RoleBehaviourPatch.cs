using AmongUs.GameOptions;
using BepInEx;
using BepInEx.Unity.IL2CPP;
using FungleAPI.Base.Roles;
using FungleAPI.Components;
using FungleAPI.Networking;
using FungleAPI.Networking.RPCs;
using FungleAPI.Patches;
using FungleAPI.Role;
using FungleAPI.Utilities;
using HarmonyLib;
using Hazel;
using Il2CppSystem.Text;
using InnerNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Threading.Tasks;
using UnityEngine;

namespace FungleAPI.Role.Patches
{
    [HarmonyPatch(typeof(RoleBehaviour))]
    internal static class RoleBehaviourPatch
    {
        [HarmonyPatch("TeamColor", MethodType.Getter)]
        [HarmonyPrefix]
        public static bool TeamColorPrefix(RoleBehaviour __instance, ref Color __result)
        { 
            ICustomRole role = __instance.CustomRole();
            if (role != null)
            {
                if (role.ShowTeamColor)
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
        [HarmonyPatch("Initialize")]
        [HarmonyPrefix]
        public static bool InitializePrefix(RoleBehaviour __instance, [HarmonyArgument(0)] PlayerControl player)
        {
            __instance.Player = player;
            if (!player.AmOwner)
            {
                return false;
            }
            DestroyableSingleton<HudManager>.Instance.SetHudActive(player, __instance, true);
            PlayerNameColor.SetForRoleDirectly(player, __instance);
            __instance.InitializeAbilityButton();
            __instance.InitializeSecondaryAbilityButton();
            return false;
        }
    }
}
