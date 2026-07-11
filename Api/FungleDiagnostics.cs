using AmongUs.GameOptions;
using BepInEx.Unity.IL2CPP;
using FungleAPI.Networking;
using FungleAPI.PluginLoading;
using FungleAPI.Role;
using System.Collections.Generic;
using System.Linq;

namespace FungleAPI.Api
{
    public static class FungleDiagnostics
    {
        public static bool IsMiraApiLoaded => IL2CPPChainloader.Instance?.Plugins.ContainsKey("mira.api") == true;
        public static bool IsReactorLoaded => IL2CPPChainloader.Instance?.Plugins.ContainsKey("gg.reactor.api") == true;
        public static int RegisteredRoleCount => CustomRoleManager.AllRoles.Count;
        public static int RegisteredRpcCount => CustomRpcManager.AllRpc.Count;
        public static IReadOnlyList<string> RegisteredMods => ModPluginManager.AllPlugins
            .Select(plugin => plugin.LocalMod.GUID ?? plugin.ModAssembly?.GetName().Name ?? "Unknown")
            .ToList();
        public static IReadOnlyList<RoleTypes> RegisteredRoleIds => CustomRoleManager.AllRoles
            .Where(role => role != null)
            .Select(role => role.Role)
            .ToList();
    }
}
