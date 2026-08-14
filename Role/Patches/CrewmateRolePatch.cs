using FungleAPI.Modifiers;
using FungleAPI.Utilities;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FungleAPI.Role.Patches
{
    [HarmonyPatch(typeof(CrewmateRole), nameof(CrewmateRole.CanUse))]
    internal static class CrewmateRolePatch
    {
        public static void Postfix(RoleBehaviour __instance, IUsable console, ref bool __result)
        {
            if (__instance.Player != null && console.Is(out Vent _) && __instance.Player.AnyModifierForceVent())
            {
                __result = true;
            }
        }
    }
}
