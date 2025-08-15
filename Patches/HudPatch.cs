using AsmResolver.Shims;
using FungleAPI;
using FungleAPI.Role;
using FungleAPI.Roles;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace FungleAPI.Patches
{
    [HarmonyPatch(typeof(HudManager))]
    public static class HudPatch
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
                    button.Button.ToggleVisible(button.Active && isActive && role.CustomRole() != null && role.CustomRole().CachedConfiguration.Buttons != null && role.CustomRole().CachedConfiguration.Buttons.Contains(button));
                }
            }
        }
        public static AbilityButton prefab;
    }
}
