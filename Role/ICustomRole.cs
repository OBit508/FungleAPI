using AmongUs.GameOptions;
using BepInEx.Configuration;
using FungleAPI.Role.Teams;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using FungleAPI.Configuration;

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
        RoleConfig Configuration { get; }
        public RoleTypes Role => CustomRoleManager.RolesToRegister[GetType()];
        public int RoleCount => Configuration.CountAndChance.GetCount();
        public int RoleChance => Configuration.CountAndChance.GetChance();
    }
}
