using AmongUs.InnerNet.GameDataMessages;
using BepInEx.Unity.IL2CPP.Utils;
using BepInEx.Unity.IL2CPP.Utils.Collections;
using FungleAPI.GameOptions;
using FungleAPI.ModCompatibility;
using FungleAPI.ModCompatibility.ReactorSupportTemp;
using FungleAPI.Player.Networking;
using FungleAPI.PluginLoading;
using FungleAPI.Translation;
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

namespace FungleAPI.Networking.Patches
{
    [HarmonyPatch(typeof(InnerNetClient._HandleGameDataInner_d__167), nameof(InnerNetClient._HandleGameDataInner_d__167.MoveNext))]
    [HarmonyPriority(Priority.Last)]
    internal static class HandleGameDataInnerPatch
    {
        public static bool Prefix(InnerNetClient._HandleGameDataInner_d__167 __instance, ref bool __result)
        {
            MessageReader messageReader = __instance.reader;

            if (messageReader.Tag == (byte)GameDataTypes.RpcFlag)
            {
                InnerNetClient innerNetClient = __instance.__4__this;

                System.Collections.IEnumerator CoHandleMessage()
                {
                    int cnt = 0;
                    try
                    {
                        InnerNetObjectCollection innerNetObjectCollection;
                        for (;;)
                        {
                            uint num3;
                            try
                            {
                                num3 = messageReader.ReadPackedUInt32();
                            }
                            catch (Exception ex)
                            {
                                Debug.LogError(string.Format("Error in {0} try {1}, Pos:{2}/{3}: {4}", new object[] { __instance.msgNum, cnt, messageReader.Position, messageReader.Length, ex }));
                                Debug.LogError(ex.Message);
                                throw;
                            }
                            byte b = messageReader.ReadByte();

                            if (b == CustomRpcManager.CustomRpc)
                            {
                                CustomRpcManager.HandleNonInnerNetObjectRpc(messageReader);

                                yield break;
                            }

                            innerNetObjectCollection = innerNetClient.allObjects;
                            lock (innerNetObjectCollection)
                            {
                                InnerNetObject innerNetObject2;
                                if (innerNetClient.allObjects.AllObjectsFast.TryGetValue(num3, out innerNetObject2))
                                {
                                    innerNetObject2.HandleRpc(b, messageReader);
                                }
                                else if (num3 != 4294967295U && !innerNetClient.DestroyedObjects.Contains(num3))
                                {
                                    Debug.LogWarning(string.Format("Stored Msg {0} RPC {1} for ", __instance.msgNum, (RpcCalls)b) + num3.ToString());
                                    int num2 = cnt;
                                    cnt = num2 + 1;
                                    if (num2 > 10)
                                    {
                                        yield break;
                                    }
                                    messageReader.Position = 0;
                                    yield return Effects.Wait(0.1f);
                                    continue;
                                }
                            }
                            break;
                        }
                        innerNetObjectCollection = null;
                    }
                    finally
                    {
                        messageReader.Recycle();
                    }
                }

                innerNetClient.StartCoroutine(CoHandleMessage().WrapToIl2Cpp());
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

                        Rpc<RpcSendModsDisconnect>.Instance.Send(new ModsDisconnectData(missingModsText, extraModsText), SendOption.Reliable, clientId);
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
