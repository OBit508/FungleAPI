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
    class HudPatch
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
        }
        [HarmonyPatch("Update")]
        [HarmonyPrefix]
        public static void OnUpdate(HudManager __instance)
        {
            foreach (CustomAbilityButton button in CustomAbilityButton.activeButton)
            {
                button.Update();
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
            HudManager.Instance.ImpostorVentButton.gameObject.SetActive(role.CanVent() && !localPlayer.Data.IsDead && role.Role != AmongUs.GameOptions.RoleTypes.Engineer && isActive);
            HudManager.Instance.KillButton.gameObject.SetActive(role.UseKillButton() && !localPlayer.Data.IsDead && isActive);
            HudManager.Instance.SabotageButton.gameObject.SetActive(role.CanSabotage() && isActive);
            ICustomRole customRole = role as ICustomRole;
            if (customRole != null)
            {
                foreach (CustomAbilityButton button in customRole.CachedConfiguration.Buttons)
                {
                    button.Button.gameObject.SetActive(button.Active && isActive);
                }
            }
        }
        public static AbilityButton prefab;
    }
}
