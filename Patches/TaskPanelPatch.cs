using FungleAPI.Role;
using FungleAPI.Role.Teams;
using FungleAPI.Roles;
using FungleAPI.Utilities;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace FungleAPI.Patches
{
    [HarmonyPatch(typeof(TaskPanelBehaviour), "SetTaskText")]
    internal class TaskPanelPatch
    {
        public static bool Prefix(TaskPanelBehaviour __instance, [HarmonyArgument(0)] string str)
        {
            ICustomRole role = PlayerControl.LocalPlayer.Data.Role.CustomRole();
            if (role != null && role.CachedConfiguration.HintType == RoleTaskHintType.Custom)
            {
                string roleText = "";
                if (role.Team != ModdedTeam.Crewmates)
                {
                    roleText = "\n" + StringNames.FakeTasks.GetString();
                }
                str = "<color=#" + ColorUtility.ToHtmlStringRGBA(role.RoleColor) + ">" + role.RoleName.GetString() + " </color>(<color=#" + ColorUtility.ToHtmlStringRGBA(role.Team.TeamColor) + ">" + role.Team.TeamName.GetString() + "</color>)" + "<color=#" + ColorUtility.ToHtmlStringRGBA(role.RoleColor) + ">" + "\n" + role.RoleBlurMed.GetString() + roleText + "\n</color>" + str;
            }
            __instance.taskText.text = str;
            return false;
        }
    }
}
