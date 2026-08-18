using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace FungleAPI.GameModes.Patches
{
    [HarmonyPatch(typeof(TaskPanelBehaviour), nameof(TaskPanelBehaviour.Update))]
    internal static class TaskPanelBehaviourPatch
    { 
        public static bool Prefix(TaskPanelBehaviour __instance)
        {
            __instance.background.transform.localScale = (__instance.taskText.textBounds.size.x > 0f) ? new Vector3( __instance.taskText.textBounds.size.x + 0.2f, __instance.taskText.textBounds.size.y + 0.2f,1f) : Vector3.zero;
            Vector3 vector = __instance.background.sprite.bounds.extents;
            vector.y = -vector.y;
            vector = Vector3.Scale(vector, __instance.background.transform.localScale);
            __instance.background.transform.localPosition = vector;
            Vector3 vector2 = __instance.tab.sprite.bounds.extents;
            vector2 = Vector3.Scale(vector2, __instance.tab.transform.localScale);
            vector2.y = -vector2.y;
            vector2.x += vector.x * 2f;
            __instance.tab.transform.localPosition = vector2;
            if (GameManager.Instance == null)
            {
                return false;
            }

            Vector3 closed = __instance.closedPosition;
            Vector3 open = __instance.openPosition;

            if (HudManager.Instance.DangerMeter != null && HudManager.Instance.DangerMeter.isActiveAndEnabled)
            {
                closed.y = 1.6f;
                open.y = 1.6f;
            }
            else
            {
                closed.y = 0.6f;
                open.y = 0.6f;
            }

            __instance.openPosition = open;

            closed = new Vector3(-__instance.background.sprite.bounds.size.x * __instance.background.transform.localScale.x, closed.y, closed.z);

            __instance.closedPosition = closed;

            if (__instance.open)
            {
                __instance.timer = Mathf.Min(1f, __instance.timer + Time.deltaTime / __instance.animationTimeSeconds);
            }
            else
            {
                __instance.timer = Mathf.Max(0f, __instance.timer - Time.deltaTime / __instance.animationTimeSeconds);
            }

            Vector3 vector3 = new Vector3(Mathf.SmoothStep(__instance.closedPosition.x, __instance.openPosition.x, __instance.timer), Mathf.SmoothStep( __instance.closedPosition.y, __instance.openPosition.y, __instance.timer), __instance.openPosition.z);

            __instance.transform.localPosition = AspectPosition.ComputePosition( AspectPosition.EdgeAlignments.LeftTop, vector3);
            return false;
        }
    }
}
