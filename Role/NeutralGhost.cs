using AmongUs.GameOptions;
using FungleAPI.Components;
using FungleAPI.Patches;
using FungleAPI.Role.Teams;
using FungleAPI.Role;
using FungleAPI.Translation;
using FungleAPI.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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
        public RoleConfig Configuration => new RoleConfig(this) { IsGhostRole = true, HintType = RoleTaskHintType.None, Buttons = new CustomAbilityButton[] { CustomAbilityButton.Instance<HauntButton>() } };
        public override bool IsDead => true;
        public override bool DidWin(GameOverReason gameOverReason)
        {
            return CustomRoleManager.DidWin(this, gameOverReason);
        }
    }
    public class HauntButton : CustomAbilityButton
    {
        public static Minigame HauntMenu;
        public override string OverrideText => TranslationController.Instance.GetString(StringNames.HauntAbilityName);
        public override Color32 TextOutlineColor => Color.gray;
        public override bool CanUse => !HauntMenu;
        public override bool Active => PlayerControl.LocalPlayer.Data.IsDead;
        public override void Click()
        {
            HauntMenu = GameObject.Instantiate<Minigame>(RoleManager.Instance.AllRoles.FirstOrDefault(obj => obj.Role == RoleTypes.CrewmateGhost).SafeCast<CrewmateGhostRole>().HauntMenu, Button.transform);
        }
        public override Sprite ButtonSprite => RoleManager.Instance.AllRoles.FirstOrDefault(obj => obj.Role == RoleTypes.CrewmateGhost).SafeCast<CrewmateGhostRole>().Ability.Image;
    }
}
