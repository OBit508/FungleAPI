using AmongUs.InnerNet.GameDataMessages;
using BepInEx.Unity.IL2CPP.Utils;
using BepInEx.Unity.IL2CPP.Utils.Collections;
using FungleAPI.Api;
using FungleAPI.GameOptions;
using FungleAPI.GlobalPatches;
using FungleAPI.ModCompatibility;
using FungleAPI.ModCompatibility.ReactorSupportTemp;
using FungleAPI.Player.Networking;
using FungleAPI.PluginLoading;
using FungleAPI.Utilities;
using HarmonyLib;
using Hazel;
using InnerNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using static Il2CppSystem.Net.Http.Headers.Parser;
using static Il2CppSystem.Net.WebSockets.ManagedWebSocket;

namespace FungleAPI.Networking.Patches
{
    [HarmonyPatch(typeof(InnerNetClient._HandleGameDataInner_d__168), nameof(InnerNetClient._HandleGameDataInner_d__168.MoveNext))]
    [HarmonyPriority(Priority.Last)]
    internal static class HandleGameDataInnerPatch
    {
        public static System.Collections.IEnumerator CoStoreMessage(InnerNetClient innerNetClient, MessageReader messageReader, uint netId, byte callId, int msgNum)
        {
            FunglePlugin<FungleApiPlugin>.Logger.LogWarning(string.Format("Stored Msg {0} RPC {1} for ", msgNum, (RpcCalls)callId) + netId.ToString());
            for (int i = 0; i < 11; i++)
            {
                if (innerNetClient.allObjects.AllObjectsFast.TryGetValue(netId, out InnerNetObject innerNetObject))
                {
                    if (callId == byte.MaxValue)
                    {
                        ReactorCompatibility.Instance?.HandleReactorRpc(innerNetObject, messageReader);
                        yield break;
                    }
                    else if (callId == CustomRpcManager.DefaultRpc)
                    {
                        CustomRpcManager.HandleRpc(innerNetObject, messageReader);
                        yield break;
                    }
                    innerNetObject.HandleRpc(callId, messageReader);
                    yield break;
                }
                yield return new WaitForSeconds(0.1f);
            }
            FunglePlugin<FungleApiPlugin>.Logger.LogError(string.Format("Failed to read stored Msg {0} RPC {1} for ", msgNum, (RpcCalls)callId) + netId.ToString());
        }
        public static bool Prefix(InnerNetClient._HandleGameDataInner_d__168 __instance, ref bool __result)
        {
            MessageReader messageReader = __instance.reader;

            if (messageReader.Tag == (byte)GameDataTypes.RpcFlag)
            {
                InnerNetClient innerNetClient = __instance.__4__this;

                uint netId = messageReader.ReadPackedUInt32();
                byte callId = messageReader.ReadByte();
                
                if (innerNetClient.allObjects.AllObjectsFast.TryGetValue(netId, out InnerNetObject innerNetObject))
                {
                    if (callId == byte.MaxValue)
                    {
                        ReactorCompatibility.Instance?.HandleReactorRpc(innerNetObject, messageReader);

                        __result = false;
                        return false;
                    }
                    else if (callId == CustomRpcManager.DefaultRpc)
                    {
                        CustomRpcManager.HandleRpc(innerNetObject, messageReader);

                        __result = false;
                        return false;
                    }
                    innerNetObject.HandleRpc(callId, messageReader);
                }
                else if (netId != uint.MaxValue && !innerNetClient.DestroyedObjects.Contains(netId))
                {
                    innerNetClient.StartCoroutine(CoStoreMessage(innerNetClient, messageReader, netId, callId, __instance.msgNum).WrapToIl2Cpp());
                }

                __result = false;
                return false;
            }
            return true;
        }
    }
}
