using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using AmongUs.GameOptions;
using UnityEngine;
using Il2CppInterop.Runtime;
using Il2CppInterop.Runtime.Injection;
using MonoMod.Cil;
using FungleAPI.LoadMod;
using FungleAPI.MonoBehaviours;
using FungleAPI.Role;
using BepInEx.Configuration;
using System.Runtime.CompilerServices;
using FungleAPI.Translation;
using FungleAPI.Role.Teams;

namespace FungleAPI.Roles
{
    public interface ICustomRole
    {
        ModPlugin RolePlugin => ModPlugin.GetModPlugin(GetType().Assembly);
        ModdedTeam Team { get; }
        StringNames RoleName { get;}
        StringNames RoleBlur { get; }
        StringNames RoleBlurMed { get; }
        StringNames RoleBlurLong { get; }
        Color RoleColor { get; }
        int RoleCount => Count.Value;
        int RoleChance => Chance.Value;
        RoleConfig Configuration { get; }
        public RoleTypes RoleType
        {
            get
            {
                foreach ((Type role, RoleTypes type) pair in AllTypes)
                {
                    if (pair.role == GetType())
                    {
                        return pair.type;
                    }
                }
                return RoleTypes.Crewmate;
            }
        }
        public RoleConfig CachedConfig
        {
            get
            {
                foreach ((ICustomRole role, RoleConfig config) pair in AllConfigs)
                {
                    if (pair.role == this)
                    {
                        return pair.config;
                    }
                }
                AllConfigs.Add((this, Configuration));
                return Configuration;
            }
        }
        internal ConfigEntry<int> Count 
        {
            get
            {
                foreach ((ConfigEntry<int> count, ICustomRole role) pair in AllCounts)
                {
                    if (pair.role == this)
                    {
                        return pair.count;
                    }
                }
                return null;
            }
            set
            {
                if (!AllCounts.Contains((value, this)))
                {
                    AllCounts.Add((value, this));
                }
            }
        }
        internal ConfigEntry<int> Chance
        {
            get
            {
                foreach ((ConfigEntry<int> chance, ICustomRole role) pair in AllChances)
                {
                    if (pair.role == this)
                    {
                        return pair.chance;
                    }
                }
                return null;
            }
            set
            {
                if (!AllChances.Contains((value, this)))
                {
                    AllChances.Add((value, this));
                }
            }
        }
        internal static List<(Type role, RoleTypes type)> AllTypes = new List<(Type role, RoleTypes type)>();
        internal static List<(ICustomRole role, RoleConfig config)> AllConfigs = new List<(ICustomRole role, RoleConfig config)>();
        internal static List<(ConfigEntry<int> count, ICustomRole role)> AllCounts = new List<(ConfigEntry<int> count, ICustomRole role)>();
        internal static List<(ConfigEntry<int> chance, ICustomRole role)> AllChances = new List<(ConfigEntry<int> chance, ICustomRole role)>();
        internal static int id = 10;
    }
}
