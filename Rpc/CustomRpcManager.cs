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
using static Il2CppSystem.Net.WebSockets.ManagedWebSocket;

namespace FungleAPI.Rpc
{
    [HarmonyPatch]
    public static class CustomRpcManager
    {
        internal static List<RpcHelper> AllRpc = new List<RpcHelper>();
        internal static Dictionary<MessageReader, List<object>> CustomReaders = new Dictionary<MessageReader, List<object>>();
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
        internal static RpcHelper RegisterRpc(Type type, ModPlugin plugin)
        {
            RpcHelper rpc = (RpcHelper)Activator.CreateInstance(type);
            AllRpc.Add(rpc);
            plugin.BasePlugin.Log.LogInfo("Registered RPC " + type.Name);
            return rpc;
        }
        public static MessageReader CreateMessageReader(List<object> objects)
        {
            MessageReader reader = new MessageReader();
            CustomReaders.Add(reader, objects);
            return reader;
        }
        internal static List<Type> InnerNetObjectTypes { get; } = (from x in typeof(InnerNetObject).Assembly.GetTypes() where x.IsSubclassOf(typeof(InnerNetObject)) select x).ToList<Type>();
        public static IEnumerable<MethodBase> TargetMethods()
        {
            return from x in InnerNetObjectTypes
                   select x.GetMethod("HandleRpc", AccessTools.allDeclared) into m
                   where m != null
                   select m;
        }
        public static void Prefix(InnerNetObject __instance, [HarmonyArgument(0)] byte callId, [HarmonyArgument(1)] MessageReader reader)
        {
            if (callId == 70)
            {
                string rpcModName = reader.ReadString();
                string rpcId = reader.ReadString();
                foreach (RpcHelper rpc in AllRpc)
                {
                    if (ModPlugin.GetModPlugin(rpc.GetType().Assembly).ModName == rpcModName && rpcId == rpc.GetType().FullName)
                    {
                        rpc.Read(reader);
                    }
                }
            }
        }
    }
}
