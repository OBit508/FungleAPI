using FungleAPI.Base.Roles;
using FungleAPI.Role;
using FungleAPI.Role.Utilities;
using FungleAPI.Teams;
using FungleAPI.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FungleAPI.ModCompatibility.MiraSupport
{
    public static class MiraExtensions
    {
        public static bool IsMiraRole(this RoleBehaviour roleBehaviour)
        {
            return MiraCompatibility.Instance != null && MiraCompatibility.Instance.RoleExtensions.IsMiraRole(roleBehaviour);
        }
        public static bool GetTeam(RoleBehaviour roleBehaviour, out ModdedTeam result)
        {
            if (MiraCompatibility.Instance == null || !roleBehaviour.IsMiraRole())
            {
                result = null;
                return false;
            }
            result = MiraCompatibility.Instance.RoleExtensions.GetTeam(roleBehaviour);
            return true;
        }
        public static bool GetHint(RoleBehaviour roleBehaviour, out RoleHintType result)
        {
            if (MiraCompatibility.Instance == null || !roleBehaviour.IsMiraRole())
            {
                result = RoleHintType.None;
                return false;
            }
            result = MiraCompatibility.Instance.RoleExtensions.GetHint(roleBehaviour);
            return true;
        }
        public static bool CanSabotage(RoleBehaviour roleBehaviour, out bool result)
        {
            if (MiraCompatibility.Instance == null || !roleBehaviour.IsMiraRole())
            {
                result = false;
                return false;
            }
            result = MiraCompatibility.Instance.RoleExtensions.CanSabotage(roleBehaviour);
            return true;
        }
        public static bool UseKillButton(RoleBehaviour roleBehaviour, out bool result)
        {
            if (MiraCompatibility.Instance == null || !roleBehaviour.IsMiraRole())
            {
                result = false;
                return false;
            }
            result = MiraCompatibility.Instance.RoleExtensions.UseKillButton(roleBehaviour);
            return true;
        }
        public static bool CanUseVent(RoleBehaviour roleBehaviour, out bool result)
        {
            if (MiraCompatibility.Instance == null || !roleBehaviour.IsMiraRole())
            {
                result = false;
                return false;
            }
            result = MiraCompatibility.Instance.RoleExtensions.CanUseVent(roleBehaviour);
            return true;
        }
    }
}
