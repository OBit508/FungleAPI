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
        CustomRoleBehaviour RoleB => this as CustomRoleBehaviour;
        ModPlugin RolePlugin { get; }
        ModdedTeam Team { get; }
        Translator RoleName { get;}
        Translator RoleBlur { get; }
        Translator RoleBlurMed { get; }
        Translator RoleBlurLong { get; }
        Color RoleColor { get; }
        Action OnRegister { get; }
        int RoleCount => Count.Value;
        int RoleChance => Chance.Value;
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
        internal static List<(ConfigEntry<int> count, ICustomRole role)> AllCounts = new List<(ConfigEntry<int> count, ICustomRole role)>();
        internal static List<(ConfigEntry<int> chance, ICustomRole role)> AllChances = new List<(ConfigEntry<int> chance, ICustomRole role)>();
    }
}
