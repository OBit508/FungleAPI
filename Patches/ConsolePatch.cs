using FungleAPI.Role;
using FungleAPI.Utilities;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace FungleAPI.Patches
{
    [HarmonyPatch(typeof(Console))]
    internal static class ConsolePatch
    {
        [HarmonyPatch("AllowImpostor", MethodType.Getter)]
        [HarmonyPrefix]
        public static bool AllowImpostorPrefix(ref bool __result)
        {
            if (PlayerControl.LocalPlayer.Data.Role.CustomRole() != null && PlayerControl.LocalPlayer.Data.Role.CanDoTasks())
            {
                __result = true;
                return false;
            }
            return true;
        }
        [HarmonyPatch("CanUse")]
        [HarmonyPrefix]
        public static bool CanUsePrefix(Console __instance, [HarmonyArgument(0)] NetworkedPlayerInfo pc, [HarmonyArgument(1)] out bool canUse, [HarmonyArgument(2)] out bool couldUse, ref float __result)
        {
            float num = float.MaxValue;
            PlayerControl @object = pc.Object;
            Vector2 truePosition = @object.GetTruePosition();
            Vector3 position = __instance.transform.position;
            couldUse = (!pc.IsDead || (GameManager.Instance.LogicOptions.GetGhostsDoTasks() && !__instance.GhostsIgnored)) && @object.CanMove && pc.Role.CanUse(__instance.SafeCast<IUsable>()) && (!__instance.onlySameRoom || __instance.InRoom(truePosition)) && (!__instance.onlyFromBelow || truePosition.y < position.y) && __instance.FindTask(@object);
            canUse = couldUse;
            if (canUse)
            {
                num = Vector2.Distance(truePosition, __instance.transform.position);
                canUse &= num <= __instance.UsableDistance;
                if (__instance.checkWalls)
                {
                    canUse &= !PhysicsHelpers.AnythingBetween(truePosition, position, Constants.ShadowMask, false);
                }
            }
            __result = num;
            return false;
        }
    }
}
