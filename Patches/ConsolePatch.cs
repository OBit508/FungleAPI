using FungleAPI.Roles;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FungleAPI.Patches
{
    [HarmonyPatch(typeof(Console))]
    public static class ConsolePatch
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
        public static bool CanUsePrefix()
        {   
            if (!PlayerControl.LocalPlayer.Data.Role.CanDoTasks())
            {
                return false;
            }
            return true;
        }
    }
}
