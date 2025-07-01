using FungleAPI.MonoBehaviours;
using FungleAPI.Patches;
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
    public class NeutralGhost : CustomRoleBehaviour, ICustomRole
    {
        public ModdedTeam Team => ModdedTeam.Neutrals;
        public StringNames RoleName => StringNames.None;
        public StringNames RoleBlur => StringNames.None;
        public StringNames RoleBlurLong => StringNames.None;
        public StringNames RoleBlurMed => StringNames.None;
        public Color RoleColor => Color.gray;
        public override bool IsGhostRole => true;
    }
}
