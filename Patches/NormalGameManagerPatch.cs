using AmongUs.GameOptions;
using FungleAPI.Player;
using FungleAPI.Role;
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
        public static bool Prefix(GameManager __instance, RoleBehaviour impostorRole, ref DeadBody __result)
        {
            __result = __instance.deadBodyPrefab[impostorRole.GetCreatedDeadBody() == DeadBodyType.Viper ? 1 : 0];
            return false;
        }
    }
}
