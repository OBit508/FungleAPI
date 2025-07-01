using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FungleAPI.Role.Teams;
using FungleAPI.Roles;
using HarmonyLib;
using UnityEngine;

namespace FungleAPI.Patches
{
    [HarmonyPatch(typeof(TaskPanelBehaviour), "SetTaskText")]
    internal class TaskPanelPatch
    {
        public static bool Prefix(TaskPanelBehaviour __instance, [HarmonyArgument(0)] string str)
        {
            ICustomRole role = PlayerControl.LocalPlayer.Data.Role as ICustomRole;
            if (role != null && role.Role.HintType == Role.RoleTaskHintType.Custom)
            {
                string roleText = "";
                if (role.Team != ModdedTeam.Crewmates)
                {
                    roleText = "\n" + StringNames.FakeTasks.GetString();
                }
                str = "<color=#" + ColorUtility.ToHtmlStringRGBA(role.RoleColor) + ">" + role.RoleName + " </color>(<color=#" + ColorUtility.ToHtmlStringRGBA(role.Team.TeamColor) + ">" + role.Team.TeamName.GetString() + "</color>)" + "<color=#" + ColorUtility.ToHtmlStringRGBA(role.RoleColor) + ">" + "\n" + role.RoleBlur + roleText + "\n</color>" + str;
            }
            __instance.taskText.text = str;
            return false;
        }
    }
}
