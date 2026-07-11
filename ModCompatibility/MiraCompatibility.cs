using AmongUs.GameOptions;
using BepInEx.Unity.IL2CPP;
using FungleAPI.Role;

namespace FungleAPI.ModCompatibility
{
    internal static class MiraCompatibility
    {
        public static bool IsLoaded => IL2CPPChainloader.Instance?.Plugins.ContainsKey("mira.api") == true;

        public static bool IsFungleRole(RoleBehaviour role)
        {
            return role != null && role.CustomRole() != null;
        }

        public static bool IsFungleRole(RoleTypes roleType)
        {
            return CustomRoleManager.AllRoles.Exists(role => role != null && role.Role == roleType && role.CustomRole() != null);
        }

        public static bool ShouldHandleLocalRole()
        {
            return !IsLoaded || IsFungleRole(PlayerControl.LocalPlayer?.Data?.Role);
        }
    }
}
