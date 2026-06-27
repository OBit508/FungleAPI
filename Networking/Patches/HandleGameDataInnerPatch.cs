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

                MessageReader clone = messageReader.CloneReader();

                clone.ReadPackedUInt32();
                byte b = clone.ReadByte();

                if (b == 241)
                {
                    clone.Recycle();

                    messageReader.ReadPackedUInt32();
                    messageReader.ReadByte();

                    CustomRpcManager.HandleNonInnerNetObjectRpc(messageReader);

                    return false;
                }
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
