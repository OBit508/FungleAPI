using FungleAPI.LoadMod;
using FungleAPI.Patches;
using HarmonyLib;
using Hazel;
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
    internal static class CustomRpcManager
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
        internal static RpcHelper RegisterRpc(Type type, ModPlugin plugin)
        {
            RpcHelper rpc = (RpcHelper)Activator.CreateInstance(type);
            AllRpc.Add(rpc);
            plugin.BasePlugin.Log.LogInfo("Registered RPC " + type.Name);
            return rpc;
        }
        private static List<Type> InnerNetObjectTypes { get; } = (from x in typeof(InnerNetObject).Assembly.GetTypes()
                                                                  where x.IsSubclassOf(typeof(InnerNetObject)) && x != typeof(LobbyBehaviour)
                                                                  select x).ToList<Type>();
        public static IEnumerable<MethodBase> TargetMethods()
        {
            return InnerNetObjectTypes
                .Select(x => x.GetMethod("HandleRpc", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.DeclaredOnly))
                .Where(m => m != null);
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
