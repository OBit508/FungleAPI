using FungleAPI.LoadMod;
using HarmonyLib;
using Hazel;
using InnerNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Il2CppSystem.Net.WebSockets.ManagedWebSocket;
using System.Reflection;

namespace FungleAPI.Rpc
{
    [HarmonyPatch]
    internal static class HandleRpcPatch
    {
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
                int rpcId = reader.ReadInt32();
                foreach (CustomRpc rpc in CustomRpcManager.AllRpc)
                {
                    if (rpcModName == rpc.Plugin.ModName && rpcId == rpc.Id)
                    {
                        rpc.Reader = reader;
                        rpc.onReceive();
                    }
                }
            }
        }
    }
}
