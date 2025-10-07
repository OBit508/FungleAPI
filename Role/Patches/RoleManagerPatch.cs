using AmongUs.GameOptions;
using FungleAPI.Components;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FungleAPI.Utilities;
using FungleAPI.Role.Teams;

namespace FungleAPI.Role.Patches
{
    [HarmonyPatch(typeof(RoleManager))]
    internal static class RoleManagerPatch
    {
        [HarmonyPatch("SetRole")]
        [HarmonyPrefix]
        public static void SetRolePrefix(RoleManager __instance, [HarmonyArgument(0)] PlayerControl targetPlayer)
        {
            if (targetPlayer.Data.Role != null)
            {
                targetPlayer.GetComponent<PlayerHelper>().OldRole = __instance.GetRole(targetPlayer.Data.Role.Role);
            }
        }
        [HarmonyPatch("AssignRoleOnDeath")]
        [HarmonyPrefix]
        public static bool AssignRoleOnDeathPrefix(RoleManager __instance, [HarmonyArgument(0)] PlayerControl player, [HarmonyArgument(1)] bool specialRolesAllowed)
        {
            if (player == null || !player.Data.IsDead)
            {
                return false;
            }
            if (specialRolesAllowed)
            {
                TryAssignSpecialGhostRoles(player);
            }
            if (!RoleManager.IsGhostRole(player.Data.Role.Role))
            {
                player.RpcSetRole(player.Data.Role.DefaultGhostRole, false);
            }
            return false;
        }
        public static void TryAssignSpecialGhostRoles(PlayerControl player)
        {
            List<RoleTypes> validRoleTypes = new List<RoleTypes>();
            foreach (RoleTypes type in RoleManager.GhostRoles)
            {
                if (RoleManager.Instance.GetRole(type).GetTeam() == player.Data.Role.GetTeam())
                {
                    validRoleTypes.Add(type);
                }
            }
            RoleTypes roleTypes = validRoleTypes[new System.Random().Next(0, validRoleTypes.Count - 1)];
            int num = PlayerControl.AllPlayerControls.ToArray().Count((PlayerControl pc) => pc.Data.IsDead && pc.Data.RoleType == roleTypes);
            IRoleOptionsCollection roleOptions = GameOptionsManager.Instance.CurrentGameOptions.RoleOptions;
            if (AmongUsClient.Instance.NetworkMode == NetworkModes.FreePlay)
            {
                player.RpcSetRole(roleTypes, false);
                return;
            }
            if (num > roleOptions.GetNumPerGame(roleTypes))
            {
                return;
            }
            int chancePerGame = roleOptions.GetChancePerGame(roleTypes);
            if (HashRandom.Next(101) < chancePerGame)
            {
                player.RpcSetRole(roleTypes, false);
            }
        }
    }
}
