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
    [HarmonyPatch]
    public static class CustomRpcManager
    {
        internal static List<RpcHelper> AllRpc = new List<RpcHelper>();
        internal static bool SafeModEnabled;
        public static bool ModdedProtocolActive 
        { 
            get
            {
                if (ReactorSupport.DisableServerAuthority)
                {
                    return true;
                }
                if (ModdedProtocol == ModdedProtocolUsage.Never || UseSafeMode && SafeModEnabled || AmongUsClient.Instance.NetworkMode != NetworkModes.OnlineGame)
                {
                    return false;
                }
                if (ModdedProtocol != ModdedProtocolUsage.Always)
                {
                    bool isOfficial = Helpers.IsCurrentServerOfficial();
                    if ((isOfficial && ModdedProtocol != ModdedProtocolUsage.OnVanillaServers) || (!isOfficial && ModdedProtocol != ModdedProtocolUsage.OnModdedServers))
                    {
                        return false;
                    }
                }
                return true;
            } 
        }
        public static ModdedProtocolUsage ModdedProtocol = ModdedProtocolUsage.Never;
        public static bool UseSafeMode = true;
        public static T Instance<T>() where T : RpcHelper
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
        public static void SendRpc(InnerNetObject innerNetObject, Action<MessageWriter> write, SendOption sendOption = SendOption.Reliable, int targetClientId = -1)
        {
            MessageWriter writer = AmongUsClient.Instance.StartRpcImmediately(innerNetObject.NetId, 240, sendOption, targetClientId);
            write(writer);
            AmongUsClient.Instance.FinishRpcImmediately(writer);
        }
        internal static List<Type> InnerNetObjectTypes { get; } = (from x in typeof(InnerNetObject).Assembly.GetTypes() where x.IsSubclassOf(typeof(InnerNetObject)) select x).ToList<Type>();
        public static IEnumerable<MethodBase> TargetMethods()
        {
            return from x in InnerNetObjectTypes
                   select x.GetMethod("HandleRpc", AccessTools.allDeclared) into m
                   where m != null
                   select m;
        }
        public static bool Prefix(InnerNetObject __instance, [HarmonyArgument(0)] byte callId, [HarmonyArgument(1)] MessageReader messageReader)
        {
            if (callId == 240)
            {
                RpcHelper rpc = messageReader.ReadRPC();
                rpc.__handle(__instance, messageReader.ReadMessage());
                return false;
            }
            return true;
        }
        public enum ModdedProtocolUsage
        {
            Always,
            OnVanillaServers,
            OnModdedServers,
            Never
        }
        [HarmonyPatch(typeof(Constants))]
        internal static class ConstantsPatch
        {
            [HarmonyPatch("GetBroadcastVersion")]
            [HarmonyPostfix]
            public static void GetBroadcastVersionPostfix(ref int __result)
            {
                if (!ModdedProtocolActive)
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
                if (!ModdedProtocolActive)
                {
                    return true;
                }
                __result = true;
                return false;
            }
        }
    }
}
