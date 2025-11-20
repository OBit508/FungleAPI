using AmongUs.GameOptions;
using BepInEx.Configuration;
using FungleAPI.Role.Teams;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using FungleAPI.Utilities;
using FungleAPI.Configuration.Attributes;
using FungleAPI.Hud;
using FungleAPI.Configuration.Helpers;

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
        List<CustomAbilityButton> Buttons { get { return Save[GetType()].Buttons.Value; } }
        List<ModdedOption> Options { get { return Save[GetType()].Options.Value; } }
        RoleCountAndChance CountAndChance { get { return Save[GetType()].CountAndChance.Value; } }
        string ExileText(ExileController exileController)
        {
            string[] tx = StringNames.ExileTextSP.GetString().Split(' ', StringSplitOptions.RemoveEmptyEntries);
            return exileController.initData.networkedPlayer.PlayerName + " " + tx[1] + " " + tx[2] + " " + exileController.initData.networkedPlayer.Role.NiceName;
        }
        bool CanUseVent => Team == ModdedTeam.Impostors;
        bool IsAffectedByLightOnAirship => Team == ModdedTeam.Impostors;
        bool UseVanillaKillButton => Team == ModdedTeam.Impostors;
        bool CanSabotage => Team == ModdedTeam.Impostors;
        bool CompletedTasksCountForProgress => Team == ModdedTeam.Crewmates;
        bool IsGhostRole => false;
        RoleTypes GhostRole => Team == ModdedTeam.Crewmates ? RoleTypes.CrewmateGhost : (Team == ModdedTeam.Impostors ? RoleTypes.ImpostorGhost : CustomRoleManager.NeutralGhost.Role);
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
        internal static Dictionary<Type, (ChangeableValue<List<ModdedOption>> Options, ChangeableValue<RoleCountAndChance> CountAndChance, ChangeableValue<List<CustomAbilityButton>> Buttons)> Save = new();
    }
}
