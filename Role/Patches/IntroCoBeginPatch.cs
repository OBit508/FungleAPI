using FungleAPI.Event;
using FungleAPI.Event.Vanilla;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FungleAPI.Role.Patches
{
    [HarmonyPatch(typeof(IntroCutscene._CoBegin_d__35), nameof(IntroCutscene._CoBegin_d__35.MoveNext))]
    internal static class IntroCoBeginPatch
    { 
        public static void Postfix(ref bool __result)
        {
            if (!__result)
            {
                EventManager.CallEvent(new IntroEndEvent());
            }
        }
    }
}
