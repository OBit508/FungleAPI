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

namespace FungleAPI.Rpc
{
    [HarmonyPatch]
    public static class CustomRpcManager
    {
        internal static List<RpcHelper> AllRpc = new List<RpcHelper>();
        public static T GetInstance<T>() where T : RpcHelper
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
        public static RpcPair CreateRpcPair(uint NetId, SendOption sendOption = SendOption.Reliable, int targetClientId = -1)
        {
            RpcPair rpcPair = new RpcPair();
            rpcPair.Writer = AmongUsClient.Instance.StartRpcImmediately(NetId, byte.MaxValue, sendOption, targetClientId);
            rpcPair.Writer.Write(true);
            return rpcPair;
        }
        internal static RpcHelper RegisterRpc(Type type, ModPlugin plugin)
        {
            RpcHelper rpc = (RpcHelper)Activator.CreateInstance(type);
            AllRpc.Add(rpc);
            plugin.BasePlugin.Log.LogInfo("Registered RPC " + type.Name);
            return rpc;
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
                bool isPair = reader.ReadBoolean();
                if (!isPair)
                {
                    string rpcModName = reader.ReadString();
                    string rpcId = reader.ReadString();
                    foreach (RpcHelper rpc in AllRpc)
                    {
                        if (ModPlugin.GetModPlugin(rpc.GetType().Assembly).ModName == rpcModName && rpcId == rpc.GetType().FullName)
                        {
                            rpc.Handle(reader.ReadMessage());
                        }
                    }
                }
                else
                {
                    MessageReader messageReader = reader.ReadMessage();
                    int count = messageReader.ReadInt32();
                    for (int i = 0; i < count; i++)
                    {
                        string rpcModName = messageReader.ReadString();
                        string rpcId = messageReader.ReadString();
                        foreach (RpcHelper rpc in AllRpc)
                        {
                            if (ModPlugin.GetModPlugin(rpc.GetType().Assembly).ModName == rpcModName && rpcId == rpc.GetType().FullName)
                            {
                                rpc.Handle(messageReader);
                            }
                        }
                    }
                }
                return false;
            }
            return true;
        }
    }
}
