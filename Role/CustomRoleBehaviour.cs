using AmongUs.GameOptions;
using FungleAPI.Role.Teams;
using FungleAPI.Roles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace FungleAPI.Role
{
    public class CustomRoleBehaviour : RoleBehaviour
    {
        public ICustomRole IRole => this as ICustomRole;
        public virtual bool AffectedByLightOnAirship => IRole.Team == ModdedTeam.Crewmates;
        public virtual bool CanKill => IRole.Team == ModdedTeam.Impostors || UseVanillaKillButton;
        public virtual bool UseVanillaKillButton => IRole.Team == ModdedTeam.Impostors;
        public virtual bool CanUseVent => IRole.Team == ModdedTeam.Impostors;
        public virtual bool CanSabotage => IRole.Team == ModdedTeam.Impostors;
        public virtual bool TasksCountForProgress => IRole.Team == ModdedTeam.Crewmates;
        public virtual bool AffectedByComms => IRole.Team == ModdedTeam.Crewmates;
        public virtual bool IsGhostRole => false;
        public virtual Config[] Configs => new Config[] {};
        public virtual CustomAbilityButton[] Buttons => new CustomAbilityButton[] {};
        public virtual RoleTypes GhostRole => RoleTypes.Crewmate;
        public virtual RoleTaskHintType HintType => RoleTaskHintType.TaskHint;
        public virtual Color OutlineColor => NameColor;
        public virtual bool ShowTeamColorOnIntro => false;
        public override bool IsDead => IsGhostRole;
        public override void AppendTaskHint(Il2CppSystem.Text.StringBuilder taskStringBuilder)
        {
            if (HintType == RoleTaskHintType.TaskHint)
            {
                base.AppendTaskHint(taskStringBuilder);
            }
        }   
    }
}
