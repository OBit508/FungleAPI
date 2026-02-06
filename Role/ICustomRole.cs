using AmongUs.GameOptions;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using FungleAPI.Utilities;
using FungleAPI.Configuration.Attributes;
using FungleAPI.Configuration.Helpers;
using FungleAPI.Teams;
using FungleAPI.PluginLoading;
using FungleAPI.GameOver;
using FungleAPI.GameOver.Ends;

namespace FungleAPI.Role
{
    /// <summary>
    /// 
    /// </summary>
    public interface ICustomRole
    {
        /// <summary>
        /// 
        /// </summary>
        ModdedTeam Team { get; }
        /// <summary>
        /// 
        /// </summary>
        StringNames RoleName { get; }
        /// <summary>
        /// 
        /// </summary>
        StringNames RoleBlur { get; }
        /// <summary>
        /// 
        /// </summary>
        StringNames RoleBlurMed { get; }
        /// <summary>
        /// 
        /// </summary>
        Color RoleColor { get; }
        /// <summary>
        /// 
        /// </summary>
        StringNames RoleBlurLong => RoleBlurMed;
        /// <summary>
        /// 
        /// </summary>
        List<ModdedOption> Options { get { return Save[GetType()].Options.Value; } }
        /// <summary>
        /// 
        /// </summary>
        RoleCountAndChance CountAndChance { get { return Save[GetType()].CountAndChance.Value; } }
        /// <summary>
        /// 
        /// </summary>
        SabotageButtonConfig CreateSabotageConfig() => null;
        /// <summary>
        /// 
        /// </summary>
        ReportButtonConfig CreateReportConfig() => null;
        /// <summary>
        /// 
        /// </summary>
        VentButtonConfig CreateVentConfig() => null;
        /// <summary>
        /// 
        /// </summary>
        KillButtonConfig CreateKillConfig() => null;
        /// <summary>
        /// 
        /// </summary>
        LightSourceConfig CreateLightConfig() => null;
        /// <summary>
        /// 
        /// </summary>
        MiraRoleTabConfig CreateRoleTabConfig() => null;
        /// <summary>
        /// 
        /// </summary>
        RoleHintType HintType => RoleHintType.TaskHint;
        /// <summary>
        /// 
        /// </summary>
        bool CanUseVent => Team == ModdedTeam.Impostors;
        /// <summary>
        /// 
        /// </summary>
        bool IsAffectedByLightOnAirship => Team == ModdedTeam.Impostors;
        /// <summary>
        /// 
        /// </summary>
        bool UseVanillaKillButton => Team == ModdedTeam.Impostors;
        /// <summary>
        /// 
        /// </summary>
        bool CanSabotage => Team == ModdedTeam.Impostors;
        /// <summary>
        /// 
        /// </summary>
        bool CompletedTasksCountForProgress => Team == ModdedTeam.Crewmates;
        /// <summary>
        /// 
        /// </summary>
        bool IsGhostRole => false;
        /// <summary>
        /// 
        /// </summary>
        RoleTypes GhostRole => Team == ModdedTeam.Crewmates ? RoleTypes.CrewmateGhost : (Team == ModdedTeam.Impostors ? RoleTypes.ImpostorGhost : CustomRoleManager.NeutralGhost.Role);
        /// <summary>
        /// 
        /// </summary>
        List<RoleTypes> AvaibleGhostRoles => null;
        /// <summary>
        /// 
        /// </summary>
        bool ShowTeamColor => false;
        /// <summary>
        /// 
        /// </summary>
        Sprite Screenshot => null;
        /// <summary>
        /// 
        /// </summary>
        bool HideRole => false;
        /// <summary>
        /// 
        /// </summary>
        bool HideInFreeplayComputer => false;
        /// <summary>
        /// 
        /// </summary>
        int MaxRoleCount => 15;
        /// <summary>
        /// 
        /// </summary>
        Sprite IconSolid => null;
        /// <summary>
        /// 
        /// </summary>
        Sprite IconWhite => null;
        /// <summary>
        /// 
        /// </summary>
        DeadBodyType CreatedDeadBodyOnKill => DeadBodyType.Normal;
        /// <summary>
        /// 
        /// </summary>
        string NeutralWinText => "Victory of the " + RoleName.GetString();
        /// <summary>
        /// 
        /// </summary>
        public RoleTypes Role => CustomRoleManager.RolesToRegister[GetType()];
        /// <summary>
        /// 
        /// </summary>
        bool CanKill => UseVanillaKillButton;
        /// <summary>
        /// 
        /// </summary>
        public Color OutlineColor => RoleColor;
        /// <summary>
        /// 
        /// </summary>
        string ExileText(ExileController exileController)
        {
            string[] tx = StringNames.ExileTextSP.GetString().Split(' ', StringSplitOptions.RemoveEmptyEntries);
            return exileController.initData.networkedPlayer.PlayerName + " " + tx[1] + " " + tx[2] + " " + exileController.initData.networkedPlayer.Role.NiceName;
        }
        /// <summary>
        /// 
        /// </summary>
        public CustomGameOver NeutralGameOver => GameOverManager.GetGameOverInstance<NeutralGameOver>();
        internal static Dictionary<Type, (ChangeableValue<List<ModdedOption>> Options, ChangeableValue<RoleCountAndChance> CountAndChance)> Save = new();
    }
}
