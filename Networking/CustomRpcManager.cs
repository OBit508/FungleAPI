using Epic.OnlineServices.RTC;
using FungleAPI.Api;
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
        public const byte DefaultRpc = 240;
        internal static uint LastRpcId = uint.MinValue;
        internal static Dictionary<uint, RpcHelper> AllRpc = new Dictionary<uint, RpcHelper>();
        /// <summary>
        /// Returns the instance of the given type
        /// </summary>
        public static T GetRpcInstance<T>() where T : RpcHelper
        {
            foreach (RpcHelper rpc in AllRpc.Values)
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
        public static void SendRpc(InnerNetObject innerNetObject, byte callId, Action<MessageWriter> write, SendOption sendOption = SendOption.Reliable, int targetClientId = -1)
        {
            MessageWriter writer = AmongUsClient.Instance.StartRpcImmediately(innerNetObject.NetId, callId, sendOption, targetClientId);
            write(writer);
            AmongUsClient.Instance.FinishRpcImmediately(writer);
        }
        public static void RegisterRpc(Type type, ModPlugin plugin)
        {
            LastRpcId++;
            RpcHelper rpc = (RpcHelper)Activator.CreateInstance(type);
            rpc.RpcId = LastRpcId;
            CustomRpcManager.AllRpc.Add(LastRpcId, rpc);
            plugin.BasePlugin.Log.LogInfo("Registered RPC " + type.Name);
        }
        public static void HandleRpc(InnerNetObject innerNetObject, MessageReader messageReader)
        {
            try
            {
                RpcHelper rpc = messageReader.ReadRPC();

                if (rpc == null)
                {
                    FunglePlugin<FungleApiPlugin>.Instance.Log.LogError($"Rpc came null");
                }

                rpc.__handle(innerNetObject, messageReader.ReadMessage());
            }
            catch (Exception ex)
            {
                FunglePlugin<FungleApiPlugin>.Instance.Log.LogError($"Failed to read rpc, Exception: {ex.Message}");
            }
        }
    }
}
