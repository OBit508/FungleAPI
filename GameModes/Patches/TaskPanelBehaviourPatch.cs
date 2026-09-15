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
            
            return false;
        }
        public static void UpdatePos(TaskPanelBehaviour taskPanelBehaviour)
        {
            taskPanelBehaviour.background.transform.localScale = (taskPanelBehaviour.taskText.textBounds.size.x > 0f) ? new Vector3(taskPanelBehaviour.taskText.textBounds.size.x + 0.2f, taskPanelBehaviour.taskText.textBounds.size.y + 0.2f, 1f) : Vector3.zero;
            Vector3 vector = taskPanelBehaviour.background.sprite.bounds.extents;
            vector.y = -vector.y;
            vector = Vector3.Scale(vector, taskPanelBehaviour.background.transform.localScale);
            taskPanelBehaviour.background.transform.localPosition = vector;
            Vector3 vector2 = taskPanelBehaviour.tab.sprite.bounds.extents;
            vector2 = Vector3.Scale(vector2, taskPanelBehaviour.tab.transform.localScale);
            vector2.y = -vector2.y;
            vector2.x += vector.x * 2f;
            taskPanelBehaviour.tab.transform.localPosition = vector2;
            if (GameManager.Instance == null)
            {
                return;
            }

            Vector3 closed = taskPanelBehaviour.closedPosition;
            Vector3 open = taskPanelBehaviour.openPosition;

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

            taskPanelBehaviour.openPosition = open;

            closed = new Vector3(-taskPanelBehaviour.background.sprite.bounds.size.x * taskPanelBehaviour.background.transform.localScale.x, closed.y, closed.z);

            taskPanelBehaviour.closedPosition = closed;

            if (taskPanelBehaviour.open)
            {
                taskPanelBehaviour.timer = Mathf.Min(1f, taskPanelBehaviour.timer + Time.deltaTime / taskPanelBehaviour.animationTimeSeconds);
            }
            else
            {
                taskPanelBehaviour.timer = Mathf.Max(0f, taskPanelBehaviour.timer - Time.deltaTime / taskPanelBehaviour.animationTimeSeconds);
            }

            Vector3 vector3 = new Vector3(Mathf.SmoothStep(taskPanelBehaviour.closedPosition.x, taskPanelBehaviour.openPosition.x, taskPanelBehaviour.timer), Mathf.SmoothStep(taskPanelBehaviour.closedPosition.y, taskPanelBehaviour.openPosition.y, taskPanelBehaviour.timer), taskPanelBehaviour.openPosition.z);

            taskPanelBehaviour.transform.localPosition = AspectPosition.ComputePosition(AspectPosition.EdgeAlignments.LeftTop, vector3);
        }
    }
}
