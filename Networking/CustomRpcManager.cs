using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using FungleAPI.Base.Rpc;
using FungleAPI.ModCompatibility;
using FungleAPI.PluginLoading;
using FungleAPI.Translation;
using FungleAPI.Utilities;
using HarmonyLib;
using Hazel;
using Il2CppInterop.Runtime.InteropTypes.Arrays;
using InnerNet;
using UnityEngine;
using static Il2CppSystem.Globalization.CultureInfo;
using static Il2CppSystem.Net.WebSockets.ManagedWebSocket;

namespace FungleAPI.Networking
{
    /// <summary>
    /// A class that helps the rpc system to work
    /// </summary>
    [HarmonyPatch]
    public static class CustomRpcManager
    {
        internal static List<RpcHelper> AllRpc = new List<RpcHelper>();
        internal static bool SafeModEnabled;
        /// <summary>
        /// Returns whether the host authority is active
        /// </summary>
        public static bool HostAuthorityActive
        { 
            get
            {
                if (ReactorSupport.DisableServerAuthority)
                {
                    return true;
                }
                if (HostAuthority == HostAuthorityUsage.Never || UseSafeMode && SafeModEnabled || AmongUsClient.Instance.NetworkMode != NetworkModes.OnlineGame)
                {
                    return false;
                }
                if (HostAuthority != HostAuthorityUsage.Always)
                {
                    bool isOfficial = Helpers.IsCurrentServerOfficial();
                    if ((isOfficial && HostAuthority != HostAuthorityUsage.OnVanillaServers) || (!isOfficial && HostAuthority != HostAuthorityUsage.OnModdedServers))
                    {
                        return false;
                    }
                }
                return true;
            } 
        }
        public static HostAuthorityUsage HostAuthority = HostAuthorityUsage.OnVanillaServers;
        public static bool UseSafeMode = true;
        /// <summary>
        /// Returns the instance of the given type
        /// </summary>
        public static T GetRpcInstance<T>() where T : RpcHelper
        {
            foreach (RpcHelper rpc in AllRpc)
            {
                if (typeof(T) == rpc.GetType())
                {
                    return rpc.SimpleCast<T>();
                }
            }
            return null;
        }
        /// <summary>
        /// Send a rpc
        /// </summary>
        public static void SendRpc(InnerNetObject innerNetObject, Action<MessageWriter> write, SendOption sendOption = SendOption.Reliable, int targetClientId = -1)
        {
            MessageWriter writer = AmongUsClient.Instance.StartRpcImmediately(innerNetObject.NetId, 240, sendOption, targetClientId);
            write(writer);
            AmongUsClient.Instance.FinishRpcImmediately(writer);
        }
        internal static void PatchInnerNetObjects()
        {
            foreach (Type type in typeof(InnerNetObject).Assembly.GetTypes().ToList().FindAll(t => t.IsSubclassOf(typeof(InnerNetObject))))
            {
                MethodInfo methodInfo = type.GetMethod("HandleRpc");
                if (methodInfo != null)
                {
                    FungleAPIPlugin.Harmony.Patch(methodInfo, new HarmonyMethod(typeof(CustomRpcManager).GetMethod("HandleRpcPrefix", AccessTools.all)));
                }
            }
        }
        private static bool HandleRpcPrefix(InnerNetObject __instance, [HarmonyArgument(0)] byte callId, [HarmonyArgument(1)] MessageReader messageReader)
        {
            if (callId == 240)
            {
                RpcHelper rpc = messageReader.ReadRPC();
                rpc.__handle(__instance, messageReader.ReadMessage());
                return false;
            }
            return true;
        }
        [HarmonyPatch(typeof(Constants))]
        internal static class ConstantsPatch
        {
            [HarmonyPatch("GetBroadcastVersion")]
            [HarmonyPostfix]
            public static void GetBroadcastVersionPostfix(ref int __result)
            {
                if (!HostAuthorityActive)
                {
                    return;
                }
                if (__result % 50 < 25)
                {
                    __result += 25;
                }
            }
            [HarmonyPatch("IsVersionModded")]
            [HarmonyPrefix]
            public static bool IsVersionModdedPrefix(ref bool __result)
            {
                if (!HostAuthorityActive)
                {
                    return true;
                }
                __result = true;
                return false;
            }
        }
        public enum HostAuthorityUsage
        {
            Always,
            OnVanillaServers,
            OnModdedServers,
            Never
        }
    }
}
