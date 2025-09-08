using FungleAPI.Role;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FungleAPI.Patches
{
    [HarmonyPatch(typeof(MeetingHud))]
    internal static class MeetingPatch
    {
        [HarmonyPatch("Start")]
        [HarmonyPostfix]
        public static void StartPostfix(MeetingHud __instance)
        {
            foreach (CustomAbilityButton button in CustomAbilityButton.Buttons.Values)
            {
                if (button.Button != null)
                {
                    button.MeetingStart(__instance);
                }
            }
        }
    }
}
