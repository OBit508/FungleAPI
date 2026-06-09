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
using FungleAPI.Extensions;

namespace FungleAPI.Role
{
    public class NeutralGhost : CrewmateGhostRole, ICustomRole
    {
        public RoleBehaviour OldRole;
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
        public KillButtonConfig KillButton => new KillButtonConfig(out KillButtonConfig self)
        {
            CheckClick = (PlayerControl target) =>
            {
                if (self.Button.currentTarget && self.Button.currentTarget == target)
                {
                    self.DoClick();
                    return;
                }
                if (self.Button.currentTarget && self.Button.currentTarget != target && PlayerControl.LocalPlayer.Data.Role.GetPlayersInAbilityRangeSorted(new Il2CppSystem.Collections.Generic.List<PlayerControl>()).Contains(target))
                {
                    self.SetTarget(target);
                    self.DoClick();
                }
            }
        };
        public RoleConfiguration Configuration => new RoleConfiguration()
        {
            CanUseVent = false,
            IsAffectedByLightOnAirship = false,
            UseVanillaKillButton = false,
            CanSabotage = OldRole != null ? OldRole.CanSabotage() : false,
            CompletedTasksCountForProgress = false,
            GhostRole = RoleTypes.CrewmateGhost,
            NeutralWinText = OldRole.CustomRole() != null ? OldRole.CustomRole().Configuration.NeutralWinText : () => string.Format(FungleTranslation.VictoryText.GetString(), OldRole.NiceName),
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
            if (Player != null)
            {
                OldRole = RoleManager.Instance.GetRole(Player.GetComponent<PlayerHelper>().LastAliveRole);
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
