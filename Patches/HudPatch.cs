using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FungleAPI.LoadMod;
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
        }
        [HarmonyPatch("Update")]
        [HarmonyPrefix]
        public static void OnUpdate(HudManager __instance)
        {
            foreach (CustomAbilityButton button in CustomAbilityButton.buttons)
            {
                button.upd();
                button.Update();
            }
            try
            {
                HudManager.Instance.ImpostorVentButton.gameObject.SetActive(PlayerControl.LocalPlayer.Data.Role.CanVent && !PlayerControl.LocalPlayer.Data.IsDead && PlayerControl.LocalPlayer.Data.RoleType != AmongUs.GameOptions.RoleTypes.Engineer && (__instance.UseButton.isActiveAndEnabled || __instance.PetButton.isActiveAndEnabled));
                HudManager.Instance.KillButton.gameObject.SetActive(PlayerControl.LocalPlayer.Data.Role.CanUseKillButton && !PlayerControl.LocalPlayer.Data.IsDead && (__instance.UseButton.isActiveAndEnabled || __instance.PetButton.isActiveAndEnabled));
                HudManager.Instance.SabotageButton.gameObject.SetActive(PlayerControl.LocalPlayer.Data.Role.CanSabotage() && (__instance.UseButton.isActiveAndEnabled || __instance.PetButton.isActiveAndEnabled));
            }
            catch
            {
            }
        }
        public static AbilityButton prefab;
    }
}
