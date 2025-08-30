using FungleAPI.Roles;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace FungleAPI.Patches
{
    [HarmonyPatch(typeof(Console))]
    internal static class ConsolePatch
    {
        [HarmonyPatch("AllowImpostor", MethodType.Getter)]
        [HarmonyPrefix]
        public static bool AllowImpostorPrefix(ref bool __result)
        {
            __result = true;
            return false;
        }
        [HarmonyPatch("CanUse")]
        [HarmonyPrefix]
        public static bool CanUsePrefix([HarmonyArgument(0)] NetworkedPlayerInfo pc, [HarmonyArgument(1)] out bool canUse, [HarmonyArgument(2)] out bool couldUse)
        {
            bool flag = true;
            if (!pc.Role.CanDoTasks())
            {
                flag = false;
            }
            canUse = flag;
            couldUse = flag;
            return flag;
        }
    }
}
