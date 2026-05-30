using Epic.OnlineServices.RTC;
using FungleAPI.Base.Rpc;
using FungleAPI.ModCompatibility;
using FungleAPI.ModCompatibility.ReactorSupportTemp;
using FungleAPI.PluginLoading;
using FungleAPI.Translation;
using FungleAPI.Utilities;
using HarmonyLib;
using Hazel;
using Il2CppInterop.Generator.Extensions;
using Il2CppInterop.Runtime.InteropTypes.Arrays;
using InnerNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using static Il2CppSystem.Globalization.CultureInfo;
using static Il2CppSystem.Net.WebSockets.ManagedWebSocket;

namespace FungleAPI.Networking
{
    /// <summary>
    /// Magane the RPCs
    /// </summary>
    [HarmonyPatch]
    public static class CustomRpcManager
    {
        internal const string RpcIdentifier = "FungleAPINonInnerNetObjectRPC";
        internal static int LastRpcId = int.MinValue;
        internal static List<RpcHelper> AllRpc = new List<RpcHelper>();
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
        public static void SendRpc(Action<MessageWriter> write, SendOption sendOption = SendOption.Reliable, int targetClientId = -1)
        {
            MessageWriter messageWriter = MessageWriter.Get(sendOption);
            if (targetClientId < 0)
            {
                messageWriter.StartMessage(5);
                messageWriter.Write(AmongUsClient.Instance.GameId);
            }
            else
            {
                messageWriter.StartMessage(6);
                messageWriter.Write(AmongUsClient.Instance.GameId);
                messageWriter.WritePacked(targetClientId);
            }
            messageWriter.StartMessage(2);
            messageWriter.WritePacked(5);
            messageWriter.Write((byte)241);
            write(messageWriter);
            AmongUsClient.Instance.FinishRpcImmediately(messageWriter);
        }
        public static void RegisterRpc(Type type, ModPlugin plugin)
        {
            LastRpcId++;
            RpcHelper rpc = (RpcHelper)Activator.CreateInstance(type);
            rpc.RpcId = LastRpcId;
            CustomRpcManager.AllRpc.Add(rpc);
            plugin.BasePlugin.Log.LogInfo("Registered RPC " + type.Name);
        }
        internal static void PatchInnerNetObjects()
        {
            foreach (Type type in typeof(InnerNetObject).Assembly.GetTypes().ToList().FindAll(t => typeof(InnerNetObject).IsAssignableFrom(t)))
            {
                MethodInfo methodInfo = type.GetMethod("HandleRpc", BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly);
                if (methodInfo != null && !methodInfo.IsVirtual)
                {
                    FungleApiPlugin.Harmony.Patch(methodInfo, new HarmonyMethod(typeof(CustomRpcManager).GetMethod("HandleRpcPrefix", AccessTools.all)));
                }
            }
        }
        internal static void HandleNonInnerNetObjectRpc(MessageReader messageReader)
        {
            RpcHelper rpc = messageReader.ReadRPC();
            rpc.__handle(messageReader.ReadMessage());
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
            [HarmonyPriority(Priority.Last)]
            [HarmonyPostfix]
            public static void GetBroadcastVersionPostfix(ref int __result)
            {
                if (ReactorCompatibility.Instance != null || AmongUsClient.Instance.NetworkMode != NetworkModes.OnlineGame) return;

                if (__result % 50 < 25)
                {
                    __result += 25;
                }
            }
            [HarmonyPatch("IsVersionModded")]
            [HarmonyPriority(Priority.Last)]
            [HarmonyPrefix]
            public static bool IsVersionModdedPrefix(ref bool __result)
            {
                if (ReactorCompatibility.Instance != null) return true;

                __result = true;
                return false;
            }
        }
    }
}
