using AmongUs.GameOptions;
using FungleAPI.MonoBehaviours;
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
    public class RoleConfig
    {
        public RoleConfig(ICustomRole role)
        {
            AffectedByLightOnAirship = role.Team == ModdedTeam.Crewmates;
            CanKill = role.Team == ModdedTeam.Impostors || UseVanillaKillButton;
            UseVanillaKillButton = role.Team == ModdedTeam.Impostors;
            CanSabotage = role.Team == ModdedTeam.Impostors;
            TasksCountForProgress = role.Team == ModdedTeam.Crewmates;
            AffectedByComms = role.Team == ModdedTeam.Crewmates;
            Configs = new Config[] { };
            Buttons = new CustomAbilityButton[] { };
            HintType = RoleTaskHintType.TaskHint;
            OutlineColor = role.RoleColor;
            ShowTeamColor = true;
            if (role.Team == ModdedTeam.Crewmates)
            {
                GhostRole = RoleTypes.CrewmateGhost;
            }
            else if (role.Team == ModdedTeam.Impostors)
            {
                GhostRole = RoleTypes.ImpostorGhost;
            }
            GhostRole = CustomRoleManager.NeutralGhost;
        }
        public bool AffectedByLightOnAirship;
        public bool CanKill;
        public bool UseVanillaKillButton;
        public bool CanUseVent;
        public bool CanSabotage;
        public bool TasksCountForProgress;
        public bool AffectedByComms;
        public bool IsGhostRole;
        public Config[] Configs;
        public CustomAbilityButton[] Buttons;
        public RoleTypes GhostRole;
        public RoleTaskHintType HintType;
        public Color OutlineColor;
        public bool ShowTeamColor; 
    }
}
