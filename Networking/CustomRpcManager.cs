using FungleAPI.Networking.RPCs;
using FungleAPI.PluginLoading;
using FungleAPI.Utilities;
using HarmonyLib;
using Hazel;
using Il2CppInterop.Runtime.InteropTypes.Arrays;
using InnerNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using static Il2CppSystem.Net.WebSockets.ManagedWebSocket;

namespace FungleAPI.Networking
{
    [HarmonyPatch]
    public static class CustomRpcManager
    {
        internal static List<RpcHelper> AllRpc = new List<RpcHelper>();
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
        internal static List<Type> InnerNetObjectTypes { get; } = (from x in typeof(InnerNetObject).Assembly.GetTypes() where x.IsSubclassOf(typeof(InnerNetObject)) select x).ToList<Type>();
        public static IEnumerable<MethodBase> TargetMethods()
        {
            return from x in InnerNetObjectTypes
                   select x.GetMethod("HandleRpc", AccessTools.allDeclared) into m
                   where m != null
                   select m;
        }
        public static bool Prefix(InnerNetObject __instance, [HarmonyArgument(0)] byte callId, [HarmonyArgument(1)] MessageReader reader)
        {
            if (callId == byte.MaxValue)
            {
                MessageReader messageReader = reader.ReadMessage();
                string rpcModName = messageReader.ReadString();
                string rpcId = messageReader.ReadString();
                foreach (RpcHelper rpc in AllRpc)
                {
                    if (ModPluginManager.GetModPlugin(rpc.GetType().Assembly).ModName == rpcModName && rpcId == rpc.GetType().FullName)
                    {
                        rpc.Handle(messageReader);
                    }
                }
                return false;
            }
            return true;
        }
    }
}
