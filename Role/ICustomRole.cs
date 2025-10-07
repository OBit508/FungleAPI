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
using FungleAPI.Utilities;

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
        string ExileText(ExileController exileController)
        {
            string[] tx = StringNames.ExileTextSP.GetString().Split(' ', StringSplitOptions.RemoveEmptyEntries);
            return exileController.initData.networkedPlayer.PlayerName + " " + tx[1] + " " + tx[2] + " " + exileController.initData.networkedPlayer.Role.NiceName;
        }
        string NeutralWinText => "Victory of the " + RoleName.GetString();
        public RoleTypes Role => CustomRoleManager.RolesToRegister[GetType()];
        public int RoleCount => Configuration.CountAndChance.GetCount();
        public int RoleChance => Configuration.CountAndChance.GetChance();
    }
}
