using FungleAPI.Components;
using FungleAPI.Role;
using HarmonyLib;
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
            TextMeshPro lobbyWarningText = UnityEngine.Object.Instantiate(__instance.AbilityButton.buttonLabelText, __instance.transform);
            lobbyWarningText.SetOutlineColor(Color.red);
            lobbyWarningText.transform.localScale *= 3;
            lobbyWarningText.transform.localPosition = new Vector3(0, 2, -0.1f);
            lobbyWarningText.gameObject.AddComponent<LobbyWarningText>().Text = lobbyWarningText;
            lobbyWarningText.alignment = TextAlignmentOptions.Top;
            lobbyWarningText.name = "LobbyWarningText";
        }
        [HarmonyPatch("Update")]
        [HarmonyPostfix]
        public static void UpdatePostfix(HudManager __instance)
        {
            if (HudHelper.UpdateFlag != HudUpdateFlag.Never && HudHelper.UpdateFlag != HudUpdateFlag.OnSetHudActive)
            {
                PlayerControl localPlayer = PlayerControl.LocalPlayer;
                if (localPlayer != null)
                {
                    RoleBehaviour role = localPlayer.Data.Role;
                    if (role != null)
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
                }
            }
            foreach (CustomAbilityButton button in CustomAbilityButton.Buttons.Values)
            {
                if (button.Button != null && button.Button.isActiveAndEnabled)
                {
                    button.Update();
                }
            }
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
    }
}
