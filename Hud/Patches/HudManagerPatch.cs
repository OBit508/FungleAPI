using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AmongUs.Data;
using AmongUs.GameOptions;
using FungleAPI.Components;
using FungleAPI.Role;
using FungleAPI.Utilities;
using HarmonyLib;
using InnerNet;
using TMPro;
using UnityEngine;

namespace FungleAPI.Hud.Patches
{
    [HarmonyPatch(typeof(HudManager))]
    internal static class HudManagerPatch
    {
        public static TaskPanelBehaviour RoleTab;
        public static float timer;
        [HarmonyPatch("Start")]
        [HarmonyPostfix]
        public static void StartPostfix(HudManager __instance)
        {
            timer = 0;
            if (ShipStatus.Instance != null)
            {
                MapBehaviour.Instance = UnityEngine.Object.Instantiate(ShipStatus.Instance.MapPrefab, __instance.transform);
                MapBehaviour.Instance.gameObject.SetActive(false);
            }
            if (AmongUsClient.Instance.IsGameStarted && AmongUsClient.Instance.NetworkMode != NetworkModes.FreePlay)
            {
                __instance.StartCoroutine(__instance.CoShowIntro());
            }
            HudHelper.BottomRight = HudManager.Instance.AbilityButton.transform.parent;
            HudHelper.BottomLeft = GameObject.Instantiate<Transform>(HudManager.Instance.AbilityButton.transform.parent, HudManager.Instance.AbilityButton.transform.parent.parent);
            for (int i = 0; i < HudHelper.BottomLeft.childCount; i++)
            {
                GameObject.Destroy(HudHelper.BottomLeft.GetChild(i).gameObject);
            }
            GridArrange gridArrange = HudHelper.BottomLeft.GetComponent<GridArrange>();
            AspectPosition aspectPosition = HudHelper.BottomLeft.GetComponent<AspectPosition>();
            HudHelper.BottomLeft.name = "BottomLeft";
            gridArrange.Alignment = GridArrange.StartAlign.Right;
            aspectPosition.Alignment = AspectPosition.EdgeAlignments.LeftBottom;
            foreach (CustomAbilityButton button in CustomAbilityButton.Buttons.Values)
            {
                button.CreateButton();
                button.Button.ToggleVisible(false);
            }
            gridArrange.Start();
            gridArrange.ArrangeChilds();
            aspectPosition.AdjustPosition();
            __instance.ImpostorVentButton.cooldownTimerText = GameObject.Instantiate<TextMeshPro>(__instance.KillButton.cooldownTimerText, __instance.ImpostorVentButton.transform);
            __instance.ImpostorVentButton.cooldownTimerText.transform.localPosition = __instance.KillButton.cooldownTimerText.transform.localPosition;
            __instance.SabotageButton.cooldownTimerText = GameObject.Instantiate<TextMeshPro>(__instance.KillButton.cooldownTimerText, __instance.SabotageButton.transform);
            __instance.SabotageButton.cooldownTimerText.transform.localPosition = __instance.KillButton.cooldownTimerText.transform.localPosition;
            ReportButtonConfig.DefaultSprite = __instance.ReportButton.graphic.sprite;
            SabotageButtonConfig.DefaultSprite = __instance.SabotageButton.graphic.sprite;
            VentButtonConfig.DefaultSprite = __instance.ImpostorVentButton.graphic.sprite;
        }
        [HarmonyPatch("SetTouchType")]
        [HarmonyPrefix]
        public static void SetTouchTypePrefix(HudManager __instance, [HarmonyArgument(0)] ControlTypes type)
        {
            if (__instance.joystick != null)
            {
                GameObject.Destroy(__instance.joystick.SafeCast<MonoBehaviour>().gameObject);
                __instance.joystick = null;
            }
            if (__instance.joystickR != null)
            {
                GameObject.Destroy(__instance.joystickR.gameObject);
                __instance.joystickR = null;
            }
            MonoBehaviour monoBehaviour = GameObject.Instantiate<MonoBehaviour>(__instance.Joysticks[(int)type]);
            if (monoBehaviour != null)
            {
                monoBehaviour.transform.SetParent(__instance.transform, false);
                __instance.joystick = monoBehaviour.GetComponent<IVirtualJoystick>();
            }
            bool flag;
            GameOptionsManager.Instance.CurrentGameOptions.TryGetBool(BoolOptionNames.UseFlashlight, out flag);
            if (type == ControlTypes.VirtualJoystick)
            {
                if (flag)
                {
                    MonoBehaviour monoBehaviour2 = GameObject.Instantiate<MonoBehaviour>(__instance.RightVJoystick);
                    if (monoBehaviour2 != null)
                    {
                        monoBehaviour2.transform.SetParent(__instance.transform, false);
                        __instance.joystickR = monoBehaviour2.GetComponent<VirtualJoystick>();
                        __instance.joystickR.ToggleVisuals(LobbyBehaviour.Instance == null);
                        Logger.GlobalInstance.Info(string.Format("[{0}] Initializing Right Joystick for Flashlight. [Use Flashlight: {1}] [Lobby: {2}]", "HudManager", flag, LobbyBehaviour.Instance != null), null);
                    }
                }
                Vector3 pos = HudHelper.BottomLeft.localPosition;
                pos.x = monoBehaviour.transform.localPosition.x + 1.5f;
                HudHelper.BottomLeft.localPosition = pos;
            }
            __instance.SetJoystickSize(DataManager.Settings.Input.TouchJoystickSize);
        }
        [HarmonyPatch("Update")]
        [HarmonyPrefix]
        public static bool UpdatePrefix(HudManager __instance)
        {
            if (__instance.consoleUIRoot.transform.localPosition.x != __instance.consoleUIHorizontalShift)
            {
                Vector3 localPosition = __instance.consoleUIRoot.transform.localPosition;
                localPosition.x = __instance.consoleUIHorizontalShift;
                __instance.consoleUIRoot.transform.localPosition = localPosition;
            }
            if (__instance.joystickR != null && LobbyBehaviour.Instance != null)
            {
                __instance.joystickR.ToggleVisuals(false);
            }
            __instance.taskDirtyTimer += Time.deltaTime;
            if (__instance.taskDirtyTimer > 0.25f)
            {
                float num = __instance.taskDirtyTimer;
                __instance.taskDirtyTimer = 0f;
                if (!PlayerControl.LocalPlayer)
                {
                    __instance.TaskPanel.SetTaskText(string.Empty);
                    return false;
                }
                NetworkedPlayerInfo data = PlayerControl.LocalPlayer.Data;
                if (data == null)
                {
                    return false;
                }
                bool flag = data.Role != null && data.Role.IsImpostor;
                __instance.tasksString.Clear();
                if (PlayerControl.LocalPlayer.myTasks == null || PlayerControl.LocalPlayer.myTasks.Count == 0)
                {
                    __instance.tasksString.Append("None");
                }
                else
                {
                    for (int i = 0; i < PlayerControl.LocalPlayer.myTasks.Count; i++)
                    {
                        PlayerTask playerTask = PlayerControl.LocalPlayer.myTasks[i];
                        if (playerTask)
                        {
                            if (playerTask.TaskType == TaskTypes.FixComms && !flag)
                            {
                                __instance.tasksString.Clear();
                                playerTask.AppendTaskText(__instance.tasksString);
                                break;
                            }
                            playerTask.AppendTaskText(__instance.tasksString);
                        }
                    }
                    if (data.Role != null && data.Role.GetHintType() == RoleHintType.TaskHint)
                    {
                        CustomRoleManager.AppendHint(data.Role, __instance.tasksString);
                    }
                    if (GameOptionsManager.Instance.CurrentGameOptions.GameMode == GameModes.HideNSeek && ShipStatus.Instance.HideCountdown > 0f)
                    {
                        ShipStatus.Instance.HideCountdown -= num;
                        __instance.tasksString.Append("\n\n" + ((int)ShipStatus.Instance.HideCountdown).ToString());
                    }
                    __instance.tasksString.TrimEnd();
                }
                __instance.TaskPanel.SetTaskText(__instance.tasksString.ToString());
            }
            foreach (CustomAbilityButton button in CustomAbilityButton.Buttons.Values)
            {
                if (button.Button != null && button.Button.isActiveAndEnabled)
                {
                    button.Update();
                }
            }
            PlayerControl localPlayer = PlayerControl.LocalPlayer;
            if (localPlayer != null)
            {
                RoleBehaviour role = localPlayer.Data.Role;
                if (role != null)
                {
                    RoleConfigManager.KillConfig?.Update?.Invoke();
                    RoleConfigManager.VentConfig?.Update?.Invoke();
                    RoleConfigManager.SabotageConfig?.Update?.Invoke();
                    if (HudHelper.UpdateFlag != HudUpdateFlag.Never && HudHelper.UpdateFlag != HudUpdateFlag.OnSetHudActive)
                    {
                        if (HudHelper.UpdateFlag == HudUpdateFlag.Delay || HudHelper.UpdateFlag == HudUpdateFlag.DelayAndOnSetHudActive)
                        {
                            timer += Time.deltaTime;
                            if (timer >= HudHelper.UpdateDelay)
                            {
                                __instance.ImpostorVentButton.ToggleVisible(role.CanVent() && !localPlayer.Data.IsDead && role.Role != AmongUs.GameOptions.RoleTypes.Engineer && HudHelper.Active);
                                __instance.KillButton.ToggleVisible(role.UseKillButton() && !localPlayer.Data.IsDead && HudHelper.Active);
                                __instance.SabotageButton.ToggleVisible(role.CanSabotage() && HudHelper.Active);
                                foreach (CustomAbilityButton button in CustomAbilityButton.Buttons.Values)
                                {
                                    if (button.Button != null)
                                    {
                                        button.Button.ToggleVisible(button.Active && HudHelper.Active);
                                    }
                                }
                                timer = 0;
                            }
                        }
                        else if (HudHelper.UpdateFlag == HudUpdateFlag.Always)
                        {
                            __instance.ImpostorVentButton.ToggleVisible(role.CanVent() && !localPlayer.Data.IsDead && role.Role != AmongUs.GameOptions.RoleTypes.Engineer && HudHelper.Active);
                            __instance.KillButton.ToggleVisible(role.UseKillButton() && !localPlayer.Data.IsDead && HudHelper.Active);
                            __instance.SabotageButton.ToggleVisible(role.CanSabotage() && HudHelper.Active);
                            foreach (CustomAbilityButton button in CustomAbilityButton.Buttons.Values)
                            {
                                if (button.Button != null)
                                {
                                    button.Button.ToggleVisible(button.Active && HudHelper.Active);
                                }
                            }
                        }
                    }
                    if (AmongUsClient.Instance.GameState != InnerNetClient.GameStates.Started && !ShipStatus.Instance)
                    {
                        return false;
                    }
                    if (role.GetHintType() == RoleHintType.MiraAPI_RoleTab)
                    {
                        if (RoleTab == null)
                        {
                            RoleTab = HudManagerPatch.CreateRoleTab(role);
                        }
                        UpdateRoleTab(role);
                        return false;
                    }
                    if (RoleTab != null)
                    {
                        GameObject.Destroy(HudManagerPatch.RoleTab.gameObject);
                    }
                }
            }
            return false;
        }
        [HarmonyPostfix]
        [HarmonyPatch("SetHudActive", new Type[]
        {
            typeof(PlayerControl),
            typeof(RoleBehaviour),
            typeof(bool)
        })]
        public static void SetHudActivePostfix(HudManager __instance, PlayerControl localPlayer, RoleBehaviour role, bool isActive)
        {
            HudHelper.Active = isActive;
            if (HudHelper.UpdateFlag == HudUpdateFlag.OnSetHudActive || HudHelper.UpdateFlag == HudUpdateFlag.DelayAndOnSetHudActive)
            {
                __instance.ImpostorVentButton.ToggleVisible(role.CanVent() && !localPlayer.Data.IsDead && role.Role != AmongUs.GameOptions.RoleTypes.Engineer && isActive);
                __instance.KillButton.ToggleVisible(role.UseKillButton() && !localPlayer.Data.IsDead && isActive);
                __instance.SabotageButton.ToggleVisible(role.CanSabotage() && isActive);
                foreach (CustomAbilityButton button in CustomAbilityButton.Buttons.Values)
                {
                    if (button.Button != null)
                    {
                        button.Button.ToggleVisible(button.Active && isActive);
                    }
                }
            }
        }
        public static TaskPanelBehaviour CreateRoleTab(RoleBehaviour role)
        {
            TaskPanelBehaviour component = HudManager.Instance.TaskStuff.transform.FindChild("TaskPanel").gameObject.GetComponent<TaskPanelBehaviour>();
            GameObject gameObject = GameObject.Instantiate<GameObject>(component.gameObject, component.transform.parent);
            gameObject.name = "RolePanel";
            TaskPanelBehaviour component2 = gameObject.GetComponent<TaskPanelBehaviour>();
            component2.open = false;
            GameObject.Destroy(component2.tab.gameObject.GetComponentInChildren<TextTranslatorTMP>());
            component2.transform.localPosition = component.transform.localPosition - new Vector3(0f, 1f, 0f);
            return component2;
        }
        public static void UpdateRoleTab(RoleBehaviour role)
        {
            TextMeshPro componentInChildren = RoleTab.tab.gameObject.GetComponentInChildren<TextMeshPro>();
            TaskPanelBehaviour component = DestroyableSingleton<HudManager>.Instance.TaskStuff.transform.FindChild("TaskPanel").gameObject.GetComponent<TaskPanelBehaviour>();
            if (componentInChildren.text != RoleConfigManager.RoleTabConfig.TabNameText)
            {
                componentInChildren.text = RoleConfigManager.RoleTabConfig.TabNameText;
            }
            if (componentInChildren.color != RoleConfigManager.RoleTabConfig.TabNameColor)
            {
                componentInChildren.color = RoleConfigManager.RoleTabConfig.TabNameColor;
            }
            float num = component.taskText.textBounds.size.y + 1f;
            RoleTab.closedPosition = new Vector3(component.closedPosition.x, component.open ? (num + 0.2f) : 2f, component.closedPosition.z);
            RoleTab.openPosition = new Vector3(component.openPosition.x, component.open ? num : 2f, component.openPosition.z);
            Il2CppSystem.Text.StringBuilder stringBuilder = new Il2CppSystem.Text.StringBuilder();
            CustomRoleManager.AppendHint(role, stringBuilder);
            RoleTab.SetTaskText(stringBuilder.ToString());
        }

    }
}
