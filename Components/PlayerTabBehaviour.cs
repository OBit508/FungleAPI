using FungleAPI.Attributes;
using FungleAPI.Extensions;
using FungleAPI.GameOptions.Lobby;
using FungleAPI.Role;
using FungleAPI.Role.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;

namespace FungleAPI.Components
{
    [RegisterTypeInIl2Cpp]
    public class PlayerTabBehaviour : MonoBehaviour
    {
        private bool Visible = true;
        public TaskPanelBehaviour Panel;

        public TextMeshPro TabText;
        public TextMeshPro TabName;
        public void Start()
        {
            SetVisible(false);
            TabText = Panel.taskText;
            TabName = Panel.tab.transform.GetChild(0).GetComponent<TextMeshPro>();
            TabName.GetComponent<TextTranslatorTMP>().Destroy();
        }
        public void Update()
        {
            UpdatePosition();

            PlayerControl playerControl = PlayerControl.LocalPlayer;
            if (playerControl != null)
            {

                RoleBehaviour roleBehaviour = playerControl.Data.Role;
                if (roleBehaviour != null)
                {

                    SetVisible(roleBehaviour.GetHintType().HasFlag(RoleHintType.PlayerTab) && HudManager.Instance.TaskPanel.gameObject.activeSelf);
                    Il2CppSystem.Text.StringBuilder stringBuilder = new Il2CppSystem.Text.StringBuilder();
                    RoleConfigManager.PlayerTabConfig.AppendTabText(stringBuilder);
                    TabText.text = stringBuilder.ToString();
                    TabName.text = RoleConfigManager.PlayerTabConfig.TabName();
                }
            }
        }
        public void SetVisible(bool visible)
        {
            if (Visible == visible) return;
            for (int i = 0; i < transform.GetChildCount(); i++)
            {
                transform.GetChild(i).gameObject.SetActive(visible);
            }
            Visible = visible;
        }
        public void UpdatePosition()
        {
            Vector3 vector = Panel.background.sprite.bounds.extents;
            Vector3 vector2 = Panel.tab.sprite.bounds.extents;

            Panel.background.transform.localScale = (Panel.taskText.textBounds.size.x > 0f) ? new Vector3(Panel.taskText.textBounds.size.x + 0.4f, Panel.taskText.textBounds.size.y + 0.3f, 1f) : Vector3.zero;
            vector.y = -vector.y;
            vector = vector.Mul(Panel.background.transform.localScale);
            Panel.background.transform.localPosition = vector;

            vector2 = vector2.Mul(Panel.tab.transform.localScale);
            vector2.y = -vector2.y;
            vector2.x += vector.x * 2f;

            Panel.tab.transform.localPosition = vector2;

            if (!GameManager.Instance)
            {
                return;
            }

            TaskPanelBehaviour taskPanel = HudManager.Instance.TaskPanel;
            float taskHeight = taskPanel.taskText.textBounds.size.y + 1f;
            float yPos = taskPanel.open ? taskHeight : 2f;

            Panel.closedPosition = new Vector3(-Panel.background.sprite.bounds.size.x * Panel.background.transform.localScale.x, taskPanel.open ? yPos + 0.2f : yPos, Panel.closedPosition.z);
            Panel.openPosition = new Vector3(taskPanel.openPosition.x, yPos, taskPanel.openPosition.z);

            Panel.timer = Panel.open ? Mathf.Min(1f, Panel.timer + Time.deltaTime / Panel.animationTimeSeconds) : Mathf.Max(0f, Panel.timer - Time.deltaTime / Panel.animationTimeSeconds);
            Panel.transform.localPosition = AspectPosition.ComputePosition(AspectPosition.EdgeAlignments.LeftTop, new Vector3(Mathf.SmoothStep(Panel.closedPosition.x, Panel.openPosition.x, Panel.timer), Mathf.SmoothStep(Panel.closedPosition.y, Panel.openPosition.y, Panel.timer), Panel.openPosition.z));
        }
    }
}
