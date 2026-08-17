using AmongUs.Data;
using AmongUs.GameOptions;
using FungleAPI.Api;
using FungleAPI.Components;
using FungleAPI.Extensions;
using FungleAPI.GameModes;
using FungleAPI.ModCompatibility;
using FungleAPI.ModCompatibility.MiraSupport;
using FungleAPI.Modifiers;
using FungleAPI.Role;
using FungleAPI.Role.Utilities;
using FungleAPI.Utilities;
using HarmonyLib;
using InnerNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
        [HarmonyPriority(Priority.Last)]
        public static void StartPostfix(HudManager __instance)
        {
            HudHelper.Bottom.Clear();
            ReportButtonConfig.DefaultSprite = __instance.ReportButton?.graphic?.sprite;
            SabotageButtonConfig.DefaultSprite = __instance.SabotageButton?.graphic?.sprite;
            VentButtonConfig.DefaultSprite = __instance.ImpostorVentButton?.graphic?.sprite;

            timer = 0;
            if (ShipStatus.Instance != null && LevelImpostorSupport.LevelImpostorAssembly == null)
            {
                MapBehaviour.Instance = UnityEngine.Object.Instantiate(ShipStatus.Instance.MapPrefab, __instance.transform);
                MapBehaviour.Instance.gameObject.SetActive(false);
            }

            __instance.IntroPrefab.gameObject.GetOrAddComponent<IntroHelper>();


            if (MiraCompatibility.Instance == null)
            {
                HudHelper.BottomRight = HudManager.Instance.AbilityButton.transform.parent;

                HudHelper.Bottom.Add(HudHelper.BottomRight.GetComponent<AspectPosition>());

                HudHelper.BottomLeft = GameObject.Instantiate<Transform>(HudHelper.BottomRight, HudHelper.BottomRight.parent);
                while (HudHelper.BottomLeft.childCount > 0)
                {
                    Transform child = HudHelper.BottomLeft.GetChild(0);
                    child.SetParent(null, false);
                    GameObject.Destroy(child.gameObject);
                }
                GridArrange gridArrange = HudHelper.BottomLeft.GetComponent<GridArrange>();
                AspectPosition aspectPosition = HudHelper.BottomLeft.GetComponent<AspectPosition>();

                HudHelper.Bottom.Add(aspectPosition);

                HudHelper.BottomLeft.name = "BottomLeft";
                gridArrange.Alignment = GridArrange.StartAlign.Right;
                aspectPosition.Alignment = AspectPosition.EdgeAlignments.LeftBottom;
                InitializeButtons();
                gridArrange.Start();
                gridArrange.ArrangeChilds();
                aspectPosition.AdjustPosition();
            }
            else
            {
                Transform Buttons = __instance.transform.Find("Buttons");
                HudHelper.BottomRight = Buttons.Find("BottomRight");
                HudHelper.BottomLeft = Buttons.Find("BottomLeft");

                if (Constants.GetPlatformType() == Platforms.Android)
                {
                    HudHelper.BottomLeft.GetChild(0)?.gameObject.Destroy();
                }

                InitializeButtons();

                GridArrange gridArrange = HudHelper.BottomLeft.GetComponent<GridArrange>();

                gridArrange.Start();
                gridArrange.ArrangeChilds();
                HudHelper.BottomLeft.GetComponent<AspectPosition>().AdjustPosition();
            }

            __instance.gameObject.GetOrAddComponent<Updater>().fixedUpdate += delegate
            {
                if (__instance.KillButton.isActiveAndEnabled)
                {
                    RoleConfigManager.KillConfig?.FixedUpdate?.Invoke();
                }
            };

            __instance.ImpostorVentButton.cooldownTimerText = GameObject.Instantiate<TextMeshPro>(__instance.KillButton.cooldownTimerText, __instance.ImpostorVentButton.transform);
            __instance.ImpostorVentButton.cooldownTimerText.transform.localPosition = __instance.KillButton.cooldownTimerText.transform.localPosition;
            __instance.SabotageButton.cooldownTimerText = GameObject.Instantiate<TextMeshPro>(__instance.KillButton.cooldownTimerText, __instance.SabotageButton.transform);
            __instance.SabotageButton.cooldownTimerText.transform.localPosition = __instance.KillButton.cooldownTimerText.transform.localPosition;
            __instance.KillButton.SetDisabled();
            __instance.ImpostorVentButton.SetDisabled();

            CreatePlayerTab();
        }

        private static void InitializeButtons()
        {
            foreach (CustomAbilityButton button in HudHelper.Buttons.Values)
            {
                try
                {
                    button.CreateButton();
                    button.Button?.ToggleVisible(false);
                }
                catch (Exception exception)
                {
                    FungleApiPlugin.Instance.Log.LogError($"Failed to create button {button.GetType().FullName}: {exception}");
                }
            }
        }
        [HarmonyPatch("Update")]
        [HarmonyPrefix]
        public static bool UpdatePrefix(HudManager __instance)
        {
            foreach (CustomAbilityButton button in HudHelper.Buttons.Values)
            {
                try
                {
                    if (button.Button == null || !button.Button.isActiveAndEnabled) continue;

                    button.Update();
                }
                catch (Exception exception)
                {
                    FungleApiPlugin.Instance.Log.LogError($"Failed to update button {button.GetType().FullName}: {exception}");
                }
            }
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
            if (__instance.joystick != null && __instance.joystick.Is(out VirtualJoystick virtualJoystick))
            {
                Vector3 pos = HudHelper.BottomLeft.localPosition;
                pos.x = virtualJoystick.transform.localPosition.x + 1.5f;
                HudHelper.BottomLeft.localPosition = pos;
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
                    GameModeManager.GetCurrentGameMode().SetTaskPanelText(__instance);
                    if (GameManager.Instance.IsHideAndSeek())
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
                        if (ShipStatus.Instance.HideCountdown > 0f)
                        {
                            ShipStatus.Instance.HideCountdown -= num;
                            __instance.tasksString.Append("\n\n" + ((int)ShipStatus.Instance.HideCountdown).ToString());
                        }
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
                    if (__instance.SabotageButton.isActiveAndEnabled)
                    {
                        RoleConfigManager.SabotageConfig?.Update?.Invoke();
                    }
                    if (__instance.ImpostorVentButton.isActiveAndEnabled)
                    {
                        RoleConfigManager.VentConfig?.Update?.Invoke();
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

            if (!GameManager.Instance.IsHideAndSeek())
            {
                __instance.ReportButton.ToggleVisible(isActive && !role.IsDead && GameModeManager.GetCurrentGameMode().CanReportBodies() && ShipStatus.Instance != null);
            }

            __instance.KillButton.ToggleVisible((role.UseKillButton() || localPlayer.AnyModifierForceKill()) && isActive);
            __instance.SabotageButton.ToggleVisible((role.CanSabotage() || localPlayer.AnyModifierForceSabotage()) && isActive);
            __instance.ImpostorVentButton.ToggleVisible((role.CanUseVent() || localPlayer.AnyModifierForceKill()) && role.Role != RoleTypes.Engineer && isActive);

            foreach (CustomAbilityButton button in HudHelper.Buttons.Values)
            {
                button.Button.ToggleVisible(button.Active && isActive);
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
