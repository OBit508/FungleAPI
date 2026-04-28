using AmongUs.GameOptions;
using FungleAPI.GameOver;
using FungleAPI.GameOver.Ends;
using FungleAPI.Player;
using FungleAPI.Teams;
using FungleAPI.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace FungleAPI.Role
{
    public struct RoleConfiguration
    {
        /// <summary>
        /// Role hint type
        /// </summary>
        public RoleHintType HintType = RoleHintType.TaskHint;
        /// <summary>
        /// Indicates whether the role can use vents
        /// </summary>
        public bool CanUseVent;
        /// <summary>
        /// Indicates whether the role is affected by the airship light
        /// </summary>
        public bool IsAffectedByLightOnAirship;
        /// <summary>
        /// Indicates whether the role can use the vanilla kill button
        /// </summary>
        public bool UseVanillaKillButton;
        /// <summary>
        /// Indicates whether the role can sabotage
        /// </summary>
        public bool CanSabotage;
        /// <summary>
        /// Indicates whether the role can kill
        /// </summary>
        public bool CanKill;
        /// <summary>
        /// Indicates whether the role tasks count for the crew win
        /// </summary>
        public bool CompletedTasksCountForProgress;
        /// <summary>
        /// Indicates whether the role is a ghost role
        /// </summary>
        public bool IsGhostRole = false;
        /// <summary>
        /// Indicates a ghost role that this role will become upon death
        /// </summary>
        public RoleTypes GhostRole;
        /// <summary>
        /// Indicates the color that is showed on the role to represent the team color
        /// </summary>
        public Color ShowedTeamColor;
        /// <summary>
        /// Role screenshot
        /// </summary>
        public Sprite Screenshot = null;
        /// <summary>
        /// Indicates whether the role is hidden on the lobby panels
        /// </summary>
        public bool HideInLobby = false;
        /// <summary>
        /// Indicates whether the role is hidden on freeplay computer
        /// </summary>
        public bool HideInFreeplay = false;
        /// <summary>
        /// Max role count
        /// </summary>
        public int MaxRoleCount = 15;
        /// <summary>
        /// Role solid icon
        /// </summary>
        public Sprite IconSolid = null;
        /// <summary>
        /// Role white icon
        /// </summary>
        public Sprite IconWhite = null;
        /// <summary>
        /// The dead body type created when the role kill
        /// </summary>
        public DeadBodyType CreatedDeadBodyOnKill = DeadBodyType.Normal;
        /// <summary>
        /// Role neutral win text
        /// </summary>
        public string NeutralWinText;
        /// <summary>
        /// Role base outline color
        /// </summary>
        public Color OutlineColor;
        /// <summary>
        /// Role neutral game over
        /// </summary>
        public CustomGameOver NeutralGameOver = GameOverManager.GetGameOverInstance<NeutralGameOver>();
        public RoleConfiguration(ICustomRole customRole)
        {
            CanUseVent = customRole.Team == ModdedTeamManager.Impostors;
            IsAffectedByLightOnAirship = customRole.Team == ModdedTeamManager.Impostors;
            UseVanillaKillButton = customRole.Team == ModdedTeamManager.Impostors;
            CanSabotage = customRole.Team == ModdedTeamManager.Impostors;
            CompletedTasksCountForProgress = customRole.Team == ModdedTeamManager.Crewmates;
            GhostRole = customRole.Team == ModdedTeamManager.Crewmates ? RoleTypes.CrewmateGhost : (customRole.Team == ModdedTeamManager.Impostors ? RoleTypes.ImpostorGhost : CustomRoleManager.NeutralGhost.Role);
            ShowedTeamColor = customRole.RoleColor;
            NeutralWinText = "Victory of the " + customRole.RoleName.GetString();
            CanKill = UseVanillaKillButton;
            OutlineColor = customRole.RoleColor;
        }
    }
}
