using AmongUs.InnerNet.GameDataMessages;
using BepInEx.Unity.IL2CPP.Utils;
using BepInEx.Unity.IL2CPP.Utils.Collections;
using FungleAPI.Api;
using FungleAPI.GameOptions;
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
    [HarmonyPatch(typeof(InnerNetClient._HandleGameDataInner_d__167), nameof(InnerNetClient._HandleGameDataInner_d__167.MoveNext))]
    [HarmonyPriority(Priority.Last)]
    internal static class HandleGameDataInnerPatch
    {
        public static System.Collections.IEnumerator CoStoreMessage(InnerNetClient innerNetClient, MessageReader messageReader, uint netId, byte callId, int msgNum)
        {
            FunglePlugin<FungleApiPlugin>.Logger.LogWarning(string.Format("Stored Msg {0} RPC {1} for ", msgNum, (RpcCalls)callId) + netId.ToString());
            for (int i = 0; i < 10; i++)
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
                FunglePlugin<FungleApiPlugin>.Logger.LogWarning(string.Format("Failed to read stored Msg {0} RPC {1} for ", msgNum, (RpcCalls)callId) + netId.ToString() + " try: " + (i + 1));
            }
            FunglePlugin<FungleApiPlugin>.Logger.LogError(string.Format("Failed to read stored Msg {0} RPC {1} for ", msgNum, (RpcCalls)callId) + netId.ToString());
        }
        public static bool Prefix(InnerNetClient._HandleGameDataInner_d__167 __instance, ref bool __result)
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
                        return false;
                    }
                    else if (callId == CustomRpcManager.DefaultRpc)
                    {
                        CustomRpcManager.HandleRpc(innerNetObject, messageReader);
                        return false;
                    }
                    innerNetObject.HandleRpc(callId, messageReader);
                }
                else if (netId != uint.MaxValue && !innerNetClient.DestroyedObjects.Contains(netId))
                {
                    innerNetClient.StartCoroutine(CoStoreMessage(innerNetClient, messageReader, netId, callId, __instance.msgNum).WrapToIl2Cpp());
                }

                return false;
            }

            if (messageReader.Tag == (byte)GameDataTypes.SceneChangeFlag)
            {
                if (ReactorCompatibility.Instance != null) return true;

                InnerNetClient innerNetClient = __instance.__4__this;

                int clientId = messageReader.ReadPackedInt32();

                string sceneName = messageReader.ReadString();

                ClientData clientData = innerNetClient.FindClientById(clientId);

                if (messageReader.BytesRemaining > 0)
                {
                    int modsCount = messageReader.ReadPackedInt32();

                    (string, string, string)[] mods = new (string, string, string)[modsCount];

                    for (int i = 0; i < modsCount; i++)
                    {
                        string GUID = messageReader.ReadString();
                        string version = messageReader.ReadString();
                        string name = messageReader.ReadString();
                        mods[i] = (GUID, version, name);
                    }

                    HandShakeManager.GetMods(mods, out List<BepInMod> sameMods, out Dictionary<string, KeyValuePair<string, string>> missingMods, out List<KeyValuePair<string, string>> extraMods);

                    if (missingMods.Count > 0 || extraMods.Count > 0)
                    {
                        string missingModsText = null;
                        string extraModsText = null;
                        if (missingMods.Count > 0)
                        {
                            int i = 0;
                            foreach (KeyValuePair<string, KeyValuePair<string, string>> missingMod in missingMods)
                            {
                                missingModsText += $"{missingMod.Value.Key} v{missingMod.Value.Value}";
                                i++;

                                if (missingMods.Count > i)
                                {
                                    missingModsText += ", ";
                                }
                                else
                                {
                                    missingModsText += ".";
                                }
                            }
                        }
                        if (extraMods.Count > 0)
                        {
                            int i = 0;
                            foreach (KeyValuePair<string, string> extraMod in extraMods)
                            {
                                extraModsText += $"{extraMod.Key} v{extraMod.Value}";
                                i++;

                                if (missingMods.Count > i)
                                {
                                    extraModsText += ", ";
                                }
                                else
                                {
                                    extraModsText += ".";
                                }
                            }
                        }

                        Rpc<RpcSendModsDisconnect>.Instance.Send(new ModsDisconnectData(missingModsText, extraModsText), PlayerControl.LocalPlayer, SendOption.Reliable, clientId);
                        AmongUsClient.Instance.KickPlayer(clientId, false);
                    }
                }
                else 
                {
                    innerNetClient.StartCoroutine(HandShakeManager.CoKick(clientData, (string playerName) => string.Format(FungleTranslation.HandShakeFail_MissingAPIDisconnect.GetString(), playerName)));
                }

                innerNetClient.StartCoroutine(innerNetClient.CoOnPlayerChangedScene(clientData, sceneName));
                return false;
            }
            return true;
        }
    }
}
