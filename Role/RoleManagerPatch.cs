using AmongUs.GameOptions;
using FungleAPI.MonoBehaviours;
using FungleAPI.Patches;
using FungleAPI.Roles;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FungleAPI.Role
{
    [HarmonyPatch(typeof(RoleManager), "SetRole")]
    public class RoleManagerPatch
    {
        public static void Prefix(RoleManager __instance, [HarmonyArgument(0)] PlayerControl targetPlayer, [HarmonyArgument(1)] RoleTypes roleType)
        {
            PlayerHelper helper = targetPlayer.GetComponent<PlayerHelper>();
            helper.OldRole = __instance.GetRole(targetPlayer.Data.RoleType);
            if (targetPlayer == PlayerControl.LocalPlayer)
            {
                if (helper.OldRole.CustomRole() != null)
                {
                    foreach (CustomAbilityButton button in helper.OldRole.CustomRole().CachedConfiguration.Buttons)
                    {
                        button.Destroy();
                    }
                }
                ICustomRole role = CustomRoleManager.GetRole(roleType);
                if (role != null)
                {
                    foreach (CustomAbilityButton button in role.CachedConfiguration.Buttons)
                    {
                        button.CreateButton();
                    }
                }
            }
        }
    }
}
