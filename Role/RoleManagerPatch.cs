using AmongUs.GameOptions;
using FungleAPI.MonoBehaviours;
using FungleAPI.Patches;
using FungleAPI.Roles;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FungleAPI.Role
{
    [HarmonyPatch(typeof(RoleManager), "SetRole")]
    public class RoleManagerPatch
    {
        public static void Prefix(RoleManager __instance, [HarmonyArgument(0)] PlayerControl targetPlayer, [HarmonyArgument(1)] RoleTypes roleType)
        {
            RoleHelper helper = targetPlayer.GetComponent<RoleHelper>();
            helper.OldRole = __instance.GetRole(targetPlayer.Data.RoleType);
        }
    }
}
