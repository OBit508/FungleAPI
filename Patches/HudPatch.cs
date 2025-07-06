using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FungleAPI;
using FungleAPI.Roles;
using HarmonyLib;
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
            MapBehaviour.Instance = GameObject.Instantiate<MapBehaviour>(ShipStatus.Instance.MapPrefab, __instance.transform);
            MapBehaviour.Instance.gameObject.SetActive(false);
        }
        [HarmonyPatch("Update")]
        [HarmonyPrefix]
        public static void OnUpdate(HudManager __instance)
        {
            foreach (CustomAbilityButton button in CustomAbilityButton.activeButton)
            {
                button.upd();
                button.Update();
            }
            try
            {
                PlayerControl localPlayer = PlayerControl.LocalPlayer;
                RoleBehaviour localRole = localPlayer.Data.Role;
                bool active = (__instance.UseButton.isActiveAndEnabled || __instance.PetButton.isActiveAndEnabled);
                HudManager.Instance.ImpostorVentButton.gameObject.SetActive(localRole.CanVent() && !localPlayer.Data.IsDead && localRole.Role != AmongUs.GameOptions.RoleTypes.Engineer && active);
                HudManager.Instance.KillButton.gameObject.SetActive(localRole.UseKillButton() && !localPlayer.Data.IsDead && active);
                HudManager.Instance.SabotageButton.gameObject.SetActive(localRole.CanSabotage() && active);
            }
            catch
            {
            }
        }
        public static AbilityButton prefab;
    }
}
