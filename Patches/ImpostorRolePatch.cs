using FungleAPI.Roles;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static UnityEngine.GraphicsBuffer;

namespace FungleAPI.Patches
{
    [HarmonyPatch(typeof(ImpostorRole), "IsValidTarget")]
    internal static class ImpostorRolePatch
    {
        public static bool Prefix(ImpostorRole __instance, [HarmonyArgument(0)] NetworkedPlayerInfo target, ref bool __result)
        {
            __result = !(target == null) && !target.Disconnected && !target.IsDead && target.PlayerId != __instance.Player.PlayerId && !(target.Role == null) && !(target.Object == null) && !target.Object.inVent && !target.Object.inMovingPlat && target.Object.Visible && (target.Role.GetTeam() != __instance.GetTeam() || __instance.GetTeam().FriendlyFire);
            return false;
        }
    }
}
