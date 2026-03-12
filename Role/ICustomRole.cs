using AmongUs.GameOptions;
using FungleAPI.Configuration.Attributes;
using FungleAPI.Configuration.Helpers;
using FungleAPI.GameOver;
using FungleAPI.GameOver.Ends;
using FungleAPI.Player;
using FungleAPI.PluginLoading;
using FungleAPI.Teams;
using FungleAPI.Utilities;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using static Il2CppSystem.Net.Http.Headers.Parser;
using static Rewired.Controller;

namespace FungleAPI.Role
{
    /// <summary>
    /// The interface used to create the component of a custom role
    /// </summary>
    public interface ICustomRole
    {
        /// <summary>
        /// Role team
        /// </summary>
        ModdedTeam Team { get; }
        /// <summary>
        /// Role name
        /// </summary>
        StringNames RoleName { get; }
        /// <summary>
        /// Role blur
        /// </summary>
        StringNames RoleBlur { get; }
        /// <summary>
        /// Role blur med
        /// </summary>
        StringNames RoleBlurMed { get; }
        /// <summary>
        /// Role color
        /// </summary>
        Color RoleColor { get; }
        /// <summary>
        /// Role blur long
        /// </summary>
        StringNames RoleBlurLong => RoleBlurMed;
        /// <summary>
        /// Role options
        /// </summary>
        List<ModdedOption> Options { get { return Save[GetType()].Options.Value; } }
        /// <summary>
        /// Role count and chance
        /// </summary>
        RoleCountAndChance CountAndChance { get { return Save[GetType()].CountAndChance.Value; } }
        /// <summary>
        /// Create the Sabotage button config for the role
        /// </summary>
        SabotageButtonConfig CreateSabotageConfig() => null;
        /// <summary>
        /// Create the Report button config for the role
        /// </summary>
        ReportButtonConfig CreateReportConfig() => null;
        /// <summary>
        /// Create the Vent button config for the role
        /// </summary>
        VentButtonConfig CreateVentConfig() => null;
        /// <summary>
        /// Create the Kill button config for the role
        /// </summary>
        KillButtonConfig CreateKillConfig() => null;
        /// <summary>
        /// Create the Light source config for the role
        /// </summary>
        LightSourceConfig CreateLightConfig() => null;
        /// <summary>
        /// Create the Mira role tab config for the role
        /// </summary>
        MiraRoleTabConfig CreateRoleTabConfig() => null;
        /// <summary>
        /// Role hint type
        /// </summary>
        RoleHintType HintType => RoleHintType.TaskHint;
        /// <summary>
        /// Returns whether the role can vent
        /// </summary>
        bool CanUseVent => Team == ModdedTeam.Impostors;
        /// <summary>
        /// Returns whether the role is affected by the airship light
        /// </summary>
        bool IsAffectedByLightOnAirship => Team == ModdedTeam.Impostors;
        /// <summary>
        /// Returns whether the role can use the vanilla kill button
        /// </summary>
        bool UseVanillaKillButton => Team == ModdedTeam.Impostors;
        /// <summary>
        /// Returns whether the role can sabotage
        /// </summary>
        bool CanSabotage => Team == ModdedTeam.Impostors;
        /// <summary>
        /// Returns whether the role tasks count for the crew win
        /// </summary>
        bool CompletedTasksCountForProgress => Team == ModdedTeam.Crewmates;
        /// <summary>
        /// Returns whether the role is a ghost role
        /// </summary>
        bool IsGhostRole => false;
        /// <summary>
        /// Returns a ghost role that a role will become upon death
        /// </summary>
        RoleTypes GhostRole => Team == ModdedTeam.Crewmates ? RoleTypes.CrewmateGhost : (Team == ModdedTeam.Impostors ? RoleTypes.ImpostorGhost : CustomRoleManager.NeutralGhost.Role);
        /// <summary>
        /// Returns the list of roles that the role can become upon death
        /// </summary>
        List<RoleTypes> AvaibleGhostRoles => null;
        /// <summary>
        /// Returns whether the role color showed as the team color
        /// </summary>
        bool ShowTeamColor => false;
        /// <summary>
        /// Role screenshot
        /// </summary>
        Sprite Screenshot => null;
        /// <summary>
        /// Returns whether the role is hidden on the lobby panels
        /// </summary>
        bool HideRole => false;
        /// <summary>
        /// Returns whether the role is hidden on freeplay computer
        /// </summary>
        bool HideInFreeplayComputer => false;
        /// <summary>
        /// Max role count
        /// </summary>
        int MaxRoleCount => 15;
        /// <summary>
        /// Role solid icon
        /// </summary>
        Sprite IconSolid => null;
        /// <summary>
        /// Role white icon
        /// </summary>
        Sprite IconWhite => null;
        /// <summary>
        /// The dead body type created when the role kill
        /// </summary>
        DeadBodyType CreatedDeadBodyOnKill => DeadBodyType.Normal;
        /// <summary>
        /// Role neutral win text
        /// </summary>
        string NeutralWinText => "Victory of the " + RoleName.GetString();
        /// <summary>
        /// Role type
        /// </summary>
        public RoleTypes Role => (this as RoleBehaviour).Role;
        /// <summary>
        /// Returns whether the role can kill
        /// </summary>
        bool CanKill => UseVanillaKillButton;
        /// <summary>
        /// Role base outline color
        /// </summary>
        public Color OutlineColor => RoleColor;
        /// <summary>
        /// Role exile text
        /// </summary>
        string ExileText(ExileController exileController)
        {
            string[] tx = StringNames.ExileTextSP.GetString().Split(' ', StringSplitOptions.RemoveEmptyEntries);
            return exileController.initData.networkedPlayer.PlayerName + " " + tx[1] + " " + tx[2] + " " + exileController.initData.networkedPlayer.Role.NiceName;
        }
        /// <summary>
        /// Role neutral game over
        /// </summary>
        public CustomGameOver NeutralGameOver => GameOverManager.GetGameOverInstance<NeutralGameOver>();
        internal static Dictionary<Type, (ChangeableValue<List<ModdedOption>> Options, ChangeableValue<RoleCountAndChance> CountAndChance)> Save = new();
    }
}
