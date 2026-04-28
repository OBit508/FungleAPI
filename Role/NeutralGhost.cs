using AmongUs.GameOptions;
using FungleAPI.Components;
using FungleAPI.Patches;
using FungleAPI.Translation;
using FungleAPI.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using FungleAPI.Hud;
using FungleAPI.Base.Roles;
using FungleAPI.Teams;
using FungleAPI.Role.Utilities;

namespace FungleAPI.Role
{
    public class NeutralGhost : CrewmateGhostRole, ICustomRole
    {
        public RoleBehaviour OldRole 
        {
            get
            {
                if (Player != null)
                {
                    return Player.GetComponent<PlayerHelper>().OldRole;
                }
                return null;
            }
        }
        public ModdedTeam Team
        {
            get
            {
                if (OldRole != null)
                {
                    return OldRole.GetTeam();
                }
                return ModdedTeamManager.Neutrals;
            }
        }
        public StringNames RoleName 
        {
            get
            {
                if (OldRole != null)
                {
                    return OldRole.StringName;
                }
                return StringNames.Ghost;
            }
        }
        public StringNames RoleBlur
        {
            get
            {
                if (OldRole != null)
                {
                    return OldRole.BlurbName;
                }
                return StringNames.None;
            }
        }
        public StringNames RoleBlurLong
        {
            get
            {
                if (OldRole != null)
                {
                    return OldRole.BlurbNameLong;
                }
                return StringNames.None;
            }
        }
        public StringNames RoleBlurMed
        {
            get
            {
                if (OldRole != null)
                {
                    return OldRole.BlurbNameMed;
                }
                return StringNames.None;
            }
        }
        public Color RoleColor
        {
            get
            {
                if (OldRole != null)
                {
                    return OldRole.NameColor;
                }
                return Color.gray;
            }
        }
        public RoleConfiguration Configuration => new RoleConfiguration()
        {
            CanUseVent = false,
            IsAffectedByLightOnAirship = false,
            UseVanillaKillButton = false,
            CanSabotage = OldRole != null ? OldRole.CanSabotage() : false,
            CompletedTasksCountForProgress = false,
            GhostRole = RoleTypes.CrewmateGhost,
            ShowedTeamColor = OldRole != null ? OldRole.TeamColor : Color.gray,
            NeutralWinText = OldRole != null ? (OldRole.CustomRole() == null ? "Victory of the " + OldRole.NiceName : OldRole.CustomRole().Configuration.NeutralWinText) : "Uhhhhhh",
            CanKill = false,
            OutlineColor = OldRole != null ? OldRole.TeamColor : Color.gray
        };
        public void Start()
        {
            if (RoleManager.InstanceExists)
            {
                CrewmateGhostRole crewmateGhost = RoleManager.Instance.AllRoles.FirstOrDefault(r => r.Role == RoleTypes.CrewmateGhost).SafeCast<CrewmateGhostRole>();
                if (crewmateGhost != null)
                {
                    HauntMenu = crewmateGhost.HauntMenu;
                    Ability = crewmateGhost.Ability;
                }
            }
        }
        public override bool DidWin(GameOverReason gameOverReason)
        {
            if (OldRole != null)
            {
                return OldRole.DidWin(gameOverReason);
            }
            return false;
        }
    }
}
