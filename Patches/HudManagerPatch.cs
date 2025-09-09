
using FungleAPI;
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

namespace FungleAPI.Patches
{
    [HarmonyPatch(typeof(HudManager))]
    internal static class HudManagerPatch
    {
        [HarmonyPatch("Start")]
        [HarmonyPostfix]
        public static void OnStart(HudManager __instance)
        {
            prefab = GameObject.Instantiate<AbilityButton>(__instance.AbilityButton, __instance.transform);
            prefab.gameObject.SetActive(false);
            if (ShipStatus.Instance != null)
            {
                MapBehaviour.Instance = GameObject.Instantiate<MapBehaviour>(ShipStatus.Instance.MapPrefab, __instance.transform);
                MapBehaviour.Instance.gameObject.SetActive(false);
            }
            if (AmongUsClient.Instance.IsGameStarted && AmongUsClient.Instance.NetworkMode != NetworkModes.FreePlay)
            {
                __instance.StartCoroutine(__instance.CoShowIntro());
            }
            foreach (CustomAbilityButton button in CustomAbilityButton.Buttons.Values)
            {
                button.CreateButton();
                button.Button.ToggleVisible(false);
            }
            TextMeshPro lobbyWarningText = GameObject.Instantiate<TextMeshPro>(prefab.buttonLabelText, __instance.transform);
            lobbyWarningText.SetOutlineColor(Color.red);
            lobbyWarningText.transform.localScale *= 3;
            lobbyWarningText.transform.localPosition = new Vector3(0, 2, -0.1f);
            lobbyWarningText.gameObject.AddComponent<LobbyWarningText>().Text = lobbyWarningText;
        }
        [HarmonyPatch("Update")]
        [HarmonyPrefix]
        public static void OnUpdate(HudManager __instance)
        {
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
            __instance.ImpostorVentButton.ToggleVisible(role.CanVent() && !localPlayer.Data.IsDead && role.Role != AmongUs.GameOptions.RoleTypes.Engineer && isActive);
            __instance.KillButton.ToggleVisible(role.UseKillButton() && !localPlayer.Data.IsDead && isActive);
            __instance.SabotageButton.ToggleVisible(role.CanSabotage() && isActive);
            foreach (CustomAbilityButton button in CustomAbilityButton.Buttons.Values)
            {
                if (button.Button != null)
                {
                    button.Button.ToggleVisible(button.Active && isActive && role.CustomRole() != null && role.CustomRole().Configuration.Buttons != null && role.CustomRole().Configuration.Buttons.Contains(button));
                }
            }
        }
        public static AbilityButton prefab;
    }
}
