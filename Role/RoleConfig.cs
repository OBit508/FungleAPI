using AmongUs.GameOptions;
using BepInEx.Configuration;
using FungleAPI.Configuration;
using FungleAPI.Role.Teams;
using FungleAPI.Role;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using static Il2CppSystem.Linq.Expressions.Interpreter.CastInstruction.CastInstructionNoT;
using FungleAPI.Utilities;
using FungleAPI.Configuration.Attributes;

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
        internal List<ModdedOption> Configs;
        public CustomAbilityButton[] Buttons;
        public RoleTypes GhostRole;
        public Color OutlineColor;
        public bool ShowTeamColor;
        public DeadBodyType CreatedBodyOnKill;
        public List<GameOverReason> WinReason;
        public Sprite Screenshot;
        public RoleCountAndChance CountAndChance;
        public bool HideRole;
        public bool HideInFreeplayComputer;
        public int MaxRoleCount;
    }
}
