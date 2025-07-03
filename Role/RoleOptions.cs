using AmongUs.GameOptions;
using BepInEx.Configuration;
using FungleAPI.LoadMod;
using FungleAPI.Patches;
using FungleAPI.Roles;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using xCloud;

namespace FungleAPI.Role
{
    [HarmonyPatch(typeof(RoleOptionsCollectionV08))]
    internal static class RoleOptions
    {
        [HarmonyPrefix]
        [HarmonyPatch("GetChancePerGame")]
        public static bool GetChancePrefix([HarmonyArgument(0)] RoleTypes roleType, ref int __result)
        {
            foreach (RoleBehaviour role in CustomRoleManager.AllRoles)
            {
                if (role.Role == roleType && role.CustomRole() != null)
                {
                    __result = role.CustomRole().RoleChance.Value;
                    return false;
                }
            }
            return true;
        }
        [HarmonyPrefix]
        [HarmonyPatch("GetNumPerGame")]
        public static bool GetNumPrefix([HarmonyArgument(0)] RoleTypes roleType, ref int __result)
        {
            foreach (RoleBehaviour role in CustomRoleManager.AllRoles)
            {
                if (role.Role == roleType && role.CustomRole() != null)
                {
                    __result = role.CustomRole().RoleCount.Value;
                    return false;
                }
            }
            return true;
        }
    }
}
