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
            CanDoTasks = role.Team == ModdedTeam.Crewmates;
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
        public RoleTaskHintType HintType;
        public Color OutlineColor;
        public bool ShowTeamColor;
        public bool CanDoTasks;
        public List<GameOverReason> WinReason;
        public Sprite Screenshot;
        public int GetCount()
        {
            if (AmongUsClient.Instance.AmHost)
            {
                return localCount.Value;
            }
            else
            {
                return onlineCount;
            }
        }
        public int GetChance()
        {
            if (AmongUsClient.Instance.AmHost)
            {
                return localChance.Value;
            }
            else
            {
                return onlineChance;
            }
        }
        internal ConfigEntry<int> localCount;
        internal int onlineCount;
        internal ConfigEntry<int> localChance;
        internal int onlineChance;
    }
}
