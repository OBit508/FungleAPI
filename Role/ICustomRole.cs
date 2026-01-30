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
    public interface ICustomRole
    {
        ModdedTeam Team { get; }
        StringNames RoleName { get; }
        StringNames RoleBlur { get; }
        StringNames RoleBlurMed { get; }
        StringNames RoleBlurLong { get; }
        Color RoleColor { get; }
        List<ModdedOption> Options { get { return Save[GetType()].Options.Value; } }
        RoleCountAndChance CountAndChance { get { return Save[GetType()].CountAndChance.Value; } }
        string ExileText(ExileController exileController)
        {
            string[] tx = StringNames.ExileTextSP.GetString().Split(' ', StringSplitOptions.RemoveEmptyEntries);
            return exileController.initData.networkedPlayer.PlayerName + " " + tx[1] + " " + tx[2] + " " + exileController.initData.networkedPlayer.Role.NiceName;
        }
        SabotageButtonConfig CreateSabotageConfig() => null;
        ReportButtonConfig CreateReportConfig() => null;
        VentButtonConfig CreateVentConfig() => null;
        KillButtonConfig CreateKillConfig() => null;
        LightSourceConfig CreateLightConfig() => null;
        MiraRoleTabConfig CreateRoleTabConfig() => null;
        RoleHintType HintType => RoleHintType.TaskHint;
        bool CanUseVent => Team == ModdedTeam.Impostors;
        bool IsAffectedByLightOnAirship => Team == ModdedTeam.Impostors;
        bool UseVanillaKillButton => Team == ModdedTeam.Impostors;
        bool CanSabotage => Team == ModdedTeam.Impostors;
        bool CompletedTasksCountForProgress => Team == ModdedTeam.Crewmates;
        bool IsGhostRole => false;
        RoleTypes GhostRole => Team == ModdedTeam.Crewmates ? RoleTypes.CrewmateGhost : (Team == ModdedTeam.Impostors ? RoleTypes.ImpostorGhost : CustomRoleManager.NeutralGhost.Role);
        List<RoleTypes> AvaibleGhostRoles => null;
        bool ShowTeamColor => false;
        Sprite Screenshot => null;
        bool HideRole => false;
        bool HideInFreeplayComputer => false;
        int MaxRoleCount => 15;
        Sprite IconSolid => null;
        Sprite IconWhite => null;
        DeadBodyType CreatedDeadBodyOnKill => DeadBodyType.Normal;
        string NeutralWinText => "Victory of the " + RoleName.GetString();
        public RoleTypes Role => CustomRoleManager.RolesToRegister[GetType()];
        bool CanKill => UseVanillaKillButton;
        public Color OutlineColor => RoleColor;
        public CustomGameOver NeutralGameOver => GameOverManager.Instance<NeutralGameOver>();
        internal static Dictionary<Type, (ChangeableValue<List<ModdedOption>> Options, ChangeableValue<RoleCountAndChance> CountAndChance)> Save = new();
    }
}
