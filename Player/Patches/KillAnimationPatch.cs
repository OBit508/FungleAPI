using BepInEx.Unity.IL2CPP.Utils.Collections;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FungleAPI.Player.Patches
{
    [HarmonyPatch(typeof(KillAnimation), "CoPerformKill")]
    internal static class KillAnimationPatch
    {
        public static bool Prefix(KillAnimation __instance, PlayerControl source, PlayerControl target, ref Il2CppSystem.Collections.IEnumerator __result)
        {
            __result = __instance.CoPerformCustomKill(source, target, MurderResultFlags.Succeeded, true, false).WrapToIl2Cpp();
            return false;
        }
    }
}
