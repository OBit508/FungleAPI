using AmongUs.GameOptions;
using BepInEx.Configuration;
using FungleAPI.Role.Teams;
using FungleAPI.Roles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

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
        public RoleConfig CachedConfiguration => cachedConfigs[GetType()];
        public int RoleCount => CachedConfiguration.GetCount();
        public int RoleChance => CachedConfiguration.GetChance();
        internal static Dictionary<Type, RoleConfig> cachedConfigs = new Dictionary<Type, RoleConfig>();
    }
}
