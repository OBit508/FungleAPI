using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AmongUs.Data;
using AmongUs.GameOptions;
using FungleAPI.Components;
using FungleAPI.ModCompatibility;
using FungleAPI.Role;
using FungleAPI.Role.Utilities;
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
        public static float timer;
        [HarmonyPatch("Start")]
        [HarmonyPostfix]
        public static void StartPostfix(HudManager __instance)
        {
            HudHelper.Bottom.Clear();

            timer = 0;
            if (ShipStatus.Instance != null && LevelImpostorSupport.LevelImpostorAssembly == null)
            {
                MapBehaviour.Instance = UnityEngine.Object.Instantiate(ShipStatus.Instance.MapPrefab, __instance.transform);
                MapBehaviour.Instance.gameObject.SetActive(false);
            }
            if (AmongUsClient.Instance.IsGameStarted && AmongUsClient.Instance.NetworkMode != NetworkModes.FreePlay)
            {
                __instance.StartCoroutine(__instance.CoShowIntro());
            }
            HudHelper.BottomRight = HudManager.Instance.AbilityButton.transform.parent;

            HudHelper.Bottom.Add(HudHelper.BottomRight.GetComponent<AspectPosition>());

            HudHelper.BottomLeft = GameObject.Instantiate<Transform>(HudHelper.BottomRight, HudHelper.BottomRight.parent);
            for (int i = 0; i < HudHelper.BottomLeft.childCount; i++)
            {
                GameObject.Destroy(HudHelper.BottomLeft.GetChild(i).gameObject);
            }
            GridArrange gridArrange = HudHelper.BottomLeft.GetComponent<GridArrange>();
            AspectPosition aspectPosition = HudHelper.BottomLeft.GetComponent<AspectPosition>();

            HudHelper.Bottom.Add(aspectPosition);

            HudHelper.BottomLeft.name = "BottomLeft";
            gridArrange.Alignment = GridArrange.StartAlign.Right;
            aspectPosition.Alignment = AspectPosition.EdgeAlignments.LeftBottom;
            foreach (CustomAbilityButton button in HudHelper.Buttons.Values)
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
            __instance.KillButton.SetDisabled();
            __instance.ImpostorVentButton.SetDisabled();

            CreatePlayerTab();
            HudHelper.SetBottomSize(0.8f);
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
                    if (data.Role != null && data.Role.GetHintType().HasFlag(RoleHintType.TaskHint))
                    {
                        RoleExtensions.AppendHint(data.Role, RoleHintType.TaskHint, __instance.tasksString);
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
            PlayerControl localPlayer = PlayerControl.LocalPlayer;
            if (localPlayer != null)
            {
                RoleBehaviour role = localPlayer.Data.Role;
                if (role != null)
                {
                    RoleConfigManager.KillConfig?.Update();
                    RoleConfigManager.VentConfig?.Update?.Invoke();
                    RoleConfigManager.SabotageConfig?.Update?.Invoke();
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
            __instance.ImpostorVentButton.ToggleVisible(role.CanUseVent() && !localPlayer.Data.IsDead && role.Role != RoleTypes.Engineer && isActive);
            __instance.KillButton.ToggleVisible(role.UseKillButton() && !localPlayer.Data.IsDead && isActive);
            __instance.SabotageButton.ToggleVisible(role.CanSabotage() && isActive);
            foreach (CustomAbilityButton button in HudHelper.Buttons.Values)
            {
                if (button.Button != null)
                {
                    button.Button.ToggleVisible(button.Active && isActive);
                }
            }
        }
        public static void CreatePlayerTab()
        {
            TaskPanelBehaviour component = HudManager.Instance.TaskPanel;
            GameObject gameObject = GameObject.Instantiate<GameObject>(component.gameObject, component.transform.parent);
            gameObject.name = "PlayerTab";
            PlayerTabBehaviour playerTabBehaviour = gameObject.AddComponent<PlayerTabBehaviour>();
            playerTabBehaviour.Panel = gameObject.GetComponent<TaskPanelBehaviour>();
            playerTabBehaviour.Panel.open = false;
            playerTabBehaviour.transform.localPosition = component.transform.localPosition - new Vector3(0f, 1f, 0f);
        }
    }
}
