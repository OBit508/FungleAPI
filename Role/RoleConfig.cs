using AmongUs.GameOptions;
using FungleAPI.Configuration;
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
            TasksCountForProgress = role.Team == ModdedTeam.Crewmates;
            CanKill = role.Team == ModdedTeam.Impostors;
            UseVanillaKillButton = role.Team == ModdedTeam.Impostors;
            CanVent = role.Team == ModdedTeam.Impostors;
            CanSabotage = role.Team == ModdedTeam.Impostors;
            Buttons = new CustomAbilityButton[] { };
            OutlineColor = role.RoleColor;
            HintType = RoleTaskHintType.Normal;
            if (role.Team == ModdedTeam.Crewmates)
            {
                GhostRole = RoleTypes.CrewmateGhost;
            }
            else if (role.Team == ModdedTeam.Impostors)
            {
                GhostRole =  RoleTypes.ImpostorGhost;
            }
            GhostRole = CustomRoleManager.GetType<NeutralGhost>();
            WinReason = role.Team.WinReason;
        }
        public bool AffectedByLightOnAirship;
        public bool CanKill;
        public bool UseVanillaKillButton;
        public bool CanVent;
        public bool CanSabotage;
        public bool TasksCountForProgress;
        public bool IsGhostRole;
        internal List<CustomConfig> Configs;
        public CustomAbilityButton[] Buttons;
        public RoleTypes GhostRole;
        public RoleTaskHintType HintType;
        public Color OutlineColor;
        public bool ShowTeamColor;
        public GameOverReason WinReason;
    }
}
