using System;
using FungleAPI.MonoBehaviours;
using HarmonyLib;
using UnityEngine;

namespace FungleAPI.Patches
{
    [HarmonyPatch(typeof(SystemConsole))]
    public static class CustomConsolePatch
    {
        [HarmonyPatch("Use")]
        [HarmonyPrefix]
        private static bool OnUse(SystemConsole __instance)
        {
            if (__instance.GetComponent<CustomConsole>() != null)
            {
                SystemConsole c = HudManager.Instance.UseButton.currentTarget.SafeCast<SystemConsole>();
                if (c != null && (Vector2.Distance(PlayerControl.LocalPlayer.transform.position, __instance.transform.position) <= __instance.UsableDistance || c == __instance) && !PhysicsHelpers.AnythingBetween(PlayerControl.LocalPlayer.Collider, PlayerControl.LocalPlayer.Collider.bounds.center, __instance.transform.position, Constants.ShipOnlyMask, false) && !Minigame.Instance)
                {
                    bool flag = true;
                    if (PlayerControl.LocalPlayer.Data.IsDead)
                    {
                        flag = __instance.GetComponent<CustomConsole>().deadsCanUse;
                    }
                    if (flag)
                    {
                        __instance.GetComponent<CustomConsole>()?.onUse();
                    }
                }
                return false;
            }
            return true;
        }

        [HarmonyPatch("CanUse")]
        [HarmonyPostfix]
        private static void OnCanUse(SystemConsole __instance, NetworkedPlayerInfo pc, ref bool canUse, ref bool couldUse, ref float __result)
        {
            try
            {
                if (__instance.GetComponent<CustomConsole>() != null)
                {
                    bool flag = true;
                    if (PlayerControl.LocalPlayer.Data.IsDead)
                    {
                        flag = __instance.GetComponent<CustomConsole>().deadsCanUse;
                    }
                    PlayerControl @object = pc.Object;
                    Vector3 center = @object.Collider.bounds.center;
                    Vector3 position = __instance.transform.position;
                    float num = Vector2.Distance(center, position);
                    canUse = num <= __instance.UsableDistance && !PhysicsHelpers.AnythingBetween(@object.Collider, center, position, Constants.ShipOnlyMask, false);
                    couldUse = flag;
                    __result = num;
                }
            }
            catch
            {
            }
        }
    }
}
