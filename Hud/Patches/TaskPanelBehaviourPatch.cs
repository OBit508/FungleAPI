using FungleAPI.Utilities;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace FungleAPI.Hud.Patches
{
    [Comment("This is from MiraAPI thanks :)")]
    [HarmonyPatch(typeof(TaskPanelBehaviour), "Update")]
    internal static class TaskPanelBehaviourPatch
    {
        public static bool Prefix(TaskPanelBehaviour __instance)
        {
            if (__instance != HudManagerPatch.RoleTab)
            {
                return true;
            }
            else
            {
                Vector3 vector = __instance.background.sprite.bounds.extents;
                Vector3 vector2 = __instance.tab.sprite.bounds.extents;
                __instance.background.transform.localScale = ((__instance.taskText.textBounds.size.x > 0f) ? new Vector3(__instance.taskText.textBounds.size.x + 0.4f, __instance.taskText.textBounds.size.y + 0.3f, 1f) : Vector3.zero);
                vector.y = -vector.y;
                vector = vector.Mul(__instance.background.transform.localScale);
                __instance.background.transform.localPosition = vector;
                vector2 = vector2.Mul(__instance.tab.transform.localScale);
                vector2.y = -vector2.y;
                vector2.x += vector.x * 2f;
                __instance.tab.transform.localPosition = vector2;
                if (GameManager.Instance == null)
                {
                    return false;
                }
                else
                {
                    __instance.closedPosition = new Vector3(-__instance.background.sprite.bounds.size.x * __instance.background.transform.localScale.x, __instance.closedPosition.y, __instance.closedPosition.z);
                    __instance.timer = (__instance.open ? Mathf.Min(1f, __instance.timer + Time.deltaTime / __instance.animationTimeSeconds) : Mathf.Max(0f, __instance.timer - Time.deltaTime / __instance.animationTimeSeconds));
                    __instance.transform.localPosition = AspectPosition.ComputePosition(AspectPosition.EdgeAlignments.LeftTop, new Vector3(Mathf.SmoothStep(__instance.closedPosition.x, __instance.openPosition.x, __instance.timer), Mathf.SmoothStep(__instance.closedPosition.y, __instance.openPosition.y, __instance.timer), __instance.openPosition.z));
                    return false;
                }
            }
        }
    }
}
