using AmongUs.InnerNet.GameDataMessages;
using BepInEx.Unity.IL2CPP.Utils;
using FungleAPI.GameOptions;
using FungleAPI.ModCompatibility;
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

namespace FungleAPI.Networking.Patches
{
    [HarmonyPatch(typeof(InnerNetClient._HandleGameDataInner_d__165), nameof(InnerNetClient._HandleGameDataInner_d__165.MoveNext))]
    internal static class HandleGameDataInnerPatch
    {
        public static bool Prefix(InnerNetClient._HandleGameDataInner_d__165 __instance, ref bool __result)
        {
            MessageReader messageReader = __instance.reader;

            if (messageReader.Tag == (byte)GameDataTypes.RpcFlag)
            {
                bool isCustom = messageReader.ReadBoolean();

                if (!isCustom) return true;

                CustomRpcManager.HandleNonInnerNetObjectRpc(messageReader);
                return false;
            }

            if (messageReader.Tag == (byte)GameDataTypes.SceneChangeFlag && ReactorSupport.ReactorAssembly == null)
            {
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

                    bool haxExtraMods = extraMods.Count > 0;

                    if (missingMods.Count > 0 || haxExtraMods)
                    {
                        StringBuilder stringBuilder = new StringBuilder();

                        if (missingMods.Count > 0)
                        {
                            stringBuilder.Append(FungleTranslation.HandShakeFail.MissingMods.GetString());
                            stringBuilder.Append(" ");

                            int i = 0;
                            foreach (KeyValuePair<string, KeyValuePair<string, string>> missingMod in missingMods)
                            {
                                stringBuilder.Append($"{missingMod.Value.Key} v{missingMod.Value.Value}");
                                i++;

                                if (missingMods.Count > i)
                                {
                                    stringBuilder.Append(", ");
                                }
                                else
                                {
                                    stringBuilder.Append(".");
                                }
                            }

                            if (haxExtraMods)
                            {
                                stringBuilder.AppendLine();
                            }
                        }

                        if (haxExtraMods)
                        {
                            stringBuilder.Append(FungleTranslation.HandShakeFail.ExtraMods.GetString());
                            stringBuilder.Append(" ");

                            int i = 0;
                            foreach (KeyValuePair<string, string> extraMod in extraMods)
                            {
                                stringBuilder.Append($"{extraMod.Key} v{extraMod.Value}");
                                i++;

                                if (missingMods.Count > i)
                                {
                                    stringBuilder.Append(", ");
                                }
                                else
                                {
                                    stringBuilder.Append(".");
                                }
                            }
                        }

                        HandShakeManager.KickWithReason(clientData.Id, stringBuilder.ToString());
                    }
                    else
                    {
                        SyncManager.RpcSyncEverything(clientData.Id);
                    }
                }
                else 
                {
                    innerNetClient.StartCoroutine(HandShakeManager.CoKick(clientData, (string playerName) => string.Format(FungleTranslation.HandShakeFail.MissingAPIDisconnect.GetString(), playerName)));
                }

                innerNetClient.StartCoroutine(innerNetClient.CoOnPlayerChangedScene(clientData, sceneName));
                return false;
            }
            return true;
        }
    }
}
