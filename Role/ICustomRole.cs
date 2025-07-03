using AmongUs.GameOptions;
using BepInEx.Configuration;
using FungleAPI.Role.Teams;
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
        public RoleTypes Role
        {
            get
            {
                foreach ((ConfigEntry<int> count, ConfigEntry<int> chance, RoleConfig config, RoleTypes type, Type role) pair in Values)
                {
                    if (pair.role == GetType())
                    {
                        return pair.type;
                    }
                }
                return RoleTypes.Crewmate;
            }
        }
        public RoleConfig CachedConfiguration
        {
            get
            {
                foreach ((ConfigEntry<int> count, ConfigEntry<int> chance, RoleConfig config, RoleTypes type, Type role) pair in Values)
                {
                    if (pair.role == GetType())
                    {
                        return pair.config;
                    }
                }
                return null;
            }
        }
        public ConfigEntry<int> RoleCount
        {
            get
            {
                foreach ((ConfigEntry<int> count, ConfigEntry<int> chance, RoleConfig config, RoleTypes type, Type role) pair in Values)
                {
                    if (pair.role == GetType())
                    {
                        return pair.count;
                    }
                }
                return null;
            }
        }
        public ConfigEntry<int> RoleChance
        {
            get
            {
                foreach ((ConfigEntry<int> count, ConfigEntry<int> chance, RoleConfig config, RoleTypes type, Type role) pair in Values)
                {
                    if (pair.role == GetType())
                    {
                        return pair.chance;
                    }
                }
                return null;
            }
        }
        internal static List<(ConfigEntry<int> count, ConfigEntry<int> chance, RoleConfig config, RoleTypes type, Type role)> Values = new List<(ConfigEntry<int> count, ConfigEntry<int> chance, RoleConfig config, RoleTypes type, Type role)>();
    }
}
