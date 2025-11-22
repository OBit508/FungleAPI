using AmongUs.GameOptions;
using FungleAPI.Role;
using FungleAPI.Utilities;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FungleAPI.Patches
{
    [HarmonyPatch(typeof(NormalGameManager), "GetDeadBody")]
    internal static class NormalGameManagerPatch
    {
        public static bool Prefix(GameManager __instance, [HarmonyArgument(0)] RoleBehaviour impostorRole, ref DeadBody __result)
        {
            __result = __instance.deadBodyPrefab[impostorRole.Role == RoleTypes.Viper || impostorRole.CustomRole() != null && impostorRole.CustomRole().CreatedDeadBodyOnKill == DeadBodyType.Viper ? 1 : 0];
            return false;
        }
    }
}
