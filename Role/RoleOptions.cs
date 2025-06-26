using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AmongUs.GameOptions;
using BepInEx.Configuration;
using FungleAPI.LoadMod;
using FungleAPI.Roles;
using HarmonyLib;
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
                if (role.Role == roleType && role as ICustomRole != null)
                {
                    __result = (role as ICustomRole).Chance.Value;
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
                if (role.Role == roleType && role as ICustomRole != null)
                {
                    __result = (role as ICustomRole).Count.Value;
                    return false;
                }
            }
            return true;
        }
    }
}
