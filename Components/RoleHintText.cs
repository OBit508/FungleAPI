using FungleAPI.Attributes;
using FungleAPI.Role;
using FungleAPI.Role.Teams;
using FungleAPI.Roles;
using FungleAPI.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace FungleAPI.Components
{
    [RegisterTypeInIl2Cpp]
    public class RoleHintText : PlayerTask
    {
        public override void AppendTaskText(Il2CppSystem.Text.StringBuilder sb)
        {
            ICustomRole role = PlayerControl.LocalPlayer.Data.Role.CustomRole();
            if (role != null && role.Configuration.HintType == RoleTaskHintType.Custom)
            {
                string roleText = "";
                if (role.Team != ModdedTeam.Crewmates)
                {
                    roleText = "\n" + StringNames.FakeTasks.GetString();
                }
                sb.AppendLine("<color=#" + ColorUtility.ToHtmlStringRGBA(role.RoleColor) + ">" + role.RoleName.GetString() + " </color>(<color=#" + ColorUtility.ToHtmlStringRGBA(role.Team.TeamColor) + ">" + role.Team.TeamName.GetString() + "</color>)" + "<color=#" + ColorUtility.ToHtmlStringRGBA(role.RoleColor) + ">" + "\n" + role.RoleBlurMed.GetString() + roleText + "</color>");
            }
        }
    }
}
