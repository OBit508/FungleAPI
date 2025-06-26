using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AmongUs.GameOptions;
using FungleAPI.Roles;
using HarmonyLib;

namespace FungleAPI.Role
{
    [HarmonyPatch(typeof(RoleManager), "SetRole")]
    public class RoleManagerPatch
    {
        public static void Postfix(RoleManager __instance, [HarmonyArgument(0)] PlayerControl targetPlayer, [HarmonyArgument(1)] RoleTypes roleType)
        {
            if (targetPlayer == PlayerControl.LocalPlayer)
            {
                foreach (RoleBehaviour r in CustomRoleManager.AllRoles)
                {
                    if (r as ICustomRole != null)
                    {
                        foreach (CustomAbilityButton button in (r as ICustomRole).RoleB.Buttons)
                        {
                            button.Destroy();
                        }
                    }

                }
                ICustomRole role = CustomRoleManager.GetRole(roleType);
                if (role != null && role.RoleB.Buttons != null)
                {
                    foreach (CustomAbilityButton button in role.RoleB.Buttons)
                    {
                        button.CreateButton();
                    }
                }
            }
        }
    }
}
