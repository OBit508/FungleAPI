using FungleAPI.MonoBehaviours;
using FungleAPI.Patches;
using FungleAPI.Role.Configuration;
using FungleAPI.Role.Teams;
using FungleAPI.Roles;
using FungleAPI.Translation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace FungleAPI.Role
{
    public class NeutralGhost : RoleBehaviour, ICustomRole
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
                return ModdedTeam.Neutrals;
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
                return Translator.GetOrCreate("Neutral Ghost").AddTranslation(SupportedLangs.Brazilian, "Fantasma Neutro").StringName;
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
        public RoleConfig Configuration => new RoleConfig(this) { IsGhostRole = true, HintType = RoleTaskHintType.None };
        public override bool IsDead => true;
        public override bool DidWin(GameOverReason gameOverReason)
        {
            return CustomRoleManager.DidWin(this, gameOverReason);
        }
    }
}
