using FungleAPI.Role.RoleEvent;
using FungleAPI.Roles;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FungleAPI.Patches
{
    [HarmonyPatch(typeof(MeetingHud))]
    public static class MeetingPatch
    {
        [HarmonyPatch("Start")]
        [HarmonyPostfix]
        public static void StartPostfix(MeetingHud __instance)
        {
            foreach (CustomAbilityButton button in CustomAbilityButton.activeButton)
            {
                button.MeetingStart(__instance);
            }
            __instance.InvokeMeetingEvent(false);
        }
        [HarmonyPatch("OnDestroy")]
        [HarmonyPrefix]
        public static void OnDestroyPrefix(MeetingHud __instance)
        {
            __instance.InvokeMeetingEvent(true);
        }
    }
}
