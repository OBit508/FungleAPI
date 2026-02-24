using AmongUs.Data;
using AmongUs.InnerNet.GameDataMessages;
using BepInEx.Core.Logging.Interpolation;
using BepInEx.Unity.IL2CPP.Utils.Collections;
using FungleAPI.ModCompatibility;
using FungleAPI.Networking.RPCs;
using FungleAPI.Patches;
using FungleAPI.PluginLoading;
using FungleAPI.Translation;
using FungleAPI.Utilities;
using HarmonyLib;
using Hazel;
using Il2CppInterop.Runtime;
using InnerNet;
using Steamworks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace FungleAPI.Networking
{
    internal static class HandShakeManager
    {
        public static DisconnectReasons MissingMods = (DisconnectReasons)230;
        public static DisconnectReasons MissingModsOnHost = (DisconnectReasons)231;
        public static DisconnectReasons NotSameMods = (DisconnectReasons)232;
        public static DisconnectReasons HostIsNotModded = (DisconnectReasons)233;
        public static DisconnectReasons FailedToVerifyMods = (DisconnectReasons)234;
        public static DisconnectReasons FailedToSyncOptions = (DisconnectReasons)235;
        public static void SendMods(MessageWriter messageWriter)
        {
            messageWriter.Write(ModPlugin.AllPlugins.Count);
            foreach (ModPlugin plugin in ModPlugin.AllPlugins)
            {
                messageWriter.Write(plugin.LocalMod.Version);
                messageWriter.Write(plugin.LocalMod.GUID);
                messageWriter.Write(plugin.LocalMod.Name);
            }
        }
        public static bool ReadModsAndCheck(MessageReader messageReader, int clientId)
        {
            AmongUsClient amongUsClient = AmongUsClient.Instance;
            bool amHost = amongUsClient.AmHost;
            try
            {
                List<string> extraMods = new List<string>();
                List<string> missingMods = new List<string>();
                List<ModPlugin.Mod> mods = new List<ModPlugin.Mod>();
                int modCount = messageReader.ReadInt32();
                for (int i = 0; i < modCount; i++)
                {
                    string version = messageReader.ReadString();
                    string guid = messageReader.ReadString();
                    string name = messageReader.ReadString();
                    ModPlugin modPlugin = ModPlugin.AllPlugins.FirstOrDefault(m => m.LocalMod.GUID == guid && m.LocalMod.Version == version);
                    ModPlugin.Mod mod = modPlugin == null ? null : modPlugin.LocalMod;
                    if (mod == null)
                    {
                        extraMods.Add(name + " (" + version + ")");
                    }
                    else
                    {
                        mods.Add(mod);
                    }
                }
                if (modCount < ModPlugin.AllPlugins.Count)
                {
                    foreach (ModPlugin plugin in ModPlugin.AllPlugins)
                    {
                        if (!mods.Contains(plugin.LocalMod))
                        {
                            missingMods.Add(plugin.LocalMod.Name + " (" + plugin.LocalMod.Version + ")");
                        }
                    }
                }
                DisconnectReasons disconnectReason = DisconnectReasons.Unknown;
                string extraText = "";
                string extraText2 = "";
                bool disconnect = false;
                if (missingMods.Count > 0 && extraMods.Count > 0)
                {
                    disconnectReason = NotSameMods;
                    extraText = string.Join(", ", missingMods) + ".";
                    extraText2 = string.Join(", ", extraMods) + ".";
                    disconnect = true;
                }
                else
                {
                    if (missingMods.Count > 0)
                    {
                        disconnectReason = amHost ? MissingMods : MissingModsOnHost;
                        extraText = string.Join(", ", missingMods) + ".";
                        disconnect = true;
                    }
                    if (extraMods.Count > 0)
                    {
                        disconnectReason = amHost ? MissingModsOnHost : MissingMods;
                        extraText = string.Join(", ", extraMods) + ".";
                        disconnect = true;
                    }
                }
                if (amHost)
                {
                    if (disconnect)
                    {
                        KickPlayer(clientId, disconnectReason, extraText, extraText2);
                        FungleAPIPlugin.Instance.Log.LogError("Removed client " + clientId);
                    }
                    if (clientId != amongUsClient.HostId)
                    {
                        Rpc<RpcSyncAllConfigs>.Instance.Send(PlayerControl.LocalPlayer, SendOption.Reliable, clientId);
                    }
                }
                else if (disconnect)
                {
                    string text = DisconnectPopup.ErrorMessages[disconnectReason].GetString();
                    if (!string.IsNullOrEmpty(extraText2) && !string.IsNullOrEmpty(extraText))
                    {
                        text = text.Replace("0", extraText) + extraText2;
                    }
                    else if (!string.IsNullOrEmpty(extraText))
                    {
                        text += extraText;
                    }
                    FungleAPIPlugin.Instance.Log.LogError("Local client disconnected");
                    amongUsClient.HandleDisconnect(DisconnectReasons.Custom, text);
                    amongUsClient.LastCustomDisconnect = text;
                }
                return disconnect;
            }
            catch (Exception ex)
            {
                FungleAPIPlugin.Instance.Log.LogError("Failed to verify mods from " + clientId.ToString() + " error: " + ex.Message);
                if (amongUsClient.AmHost)
                {
                    amongUsClient.KickPlayer(clientId, FailedToVerifyMods);
                }
                return true;
            }
        }
        public static void PatchHandShake()
        {
            FungleAPIPlugin.Harmony.Patch(typeof(InnerNetClient._CoSendSceneChange_d__156).GetMethod("MoveNext"), new HarmonyMethod(typeof(HandShakeManager).GetMethod("InnerNetClient_CoSendSceneChange_d__156_MoveNext_Prefix")));
            FungleAPIPlugin.Harmony.Patch(typeof(InnerNetClient._CoHandleSpawn_d__166).GetMethod("MoveNext"), null, new HarmonyMethod(typeof(HandShakeManager).GetMethod("InnerNetClient_CoHandleSpawn_d__166_MoveNext_Postfix")));
            FungleAPIPlugin.Harmony.Patch(typeof(InnerNetClient._HandleGameDataInner_d__165).GetMethod("MoveNext"), new HarmonyMethod(typeof(HandShakeManager).GetMethod("InnerNetClient_HandleGameDataInner_d__165_MoveNext_Prefix")));
            FungleAPIPlugin.Harmony.Patch(typeof(InnerNetClient._CoHandleSpawn_d__166).GetMethod("MoveNext"), null, new HarmonyMethod(typeof(HandShakeManager).GetMethod("SpawnGameDataMessage_SerializeValues_Postfix")));
        }
        public static void KickPlayer(int clientId, DisconnectReasons disconnectReason, string extraText = "", string extraText2 = "")
        {
            AmongUsClient.Instance.KickPlayer(clientId, disconnectReason, extraText, extraText2);
        }
        public static void KickPlayer(this InnerNetClient innerNetClient, int clientId, DisconnectReasons disconnectReason, string extraText = "", string extraText2 = "")
        {
            if (disconnectReason == DisconnectReasons.Kicked || disconnectReason == DisconnectReasons.Banned)
            {
                innerNetClient.KickPlayer(clientId, disconnectReason == DisconnectReasons.Banned);
                return;
            }
            if (innerNetClient.AmHost)
            {
                MessageWriter messageWriter = MessageWriter.Get(SendOption.Reliable);
                messageWriter.StartMessage(6);
                messageWriter.Write(innerNetClient.GameId);
                messageWriter.WritePacked(clientId);
                messageWriter.StartMessage(200);
                messageWriter.Write((byte)disconnectReason);
                messageWriter.Write(extraText);
                messageWriter.Write(extraText2);
                messageWriter.EndMessage();
                messageWriter.EndMessage();
                innerNetClient.SendOrDisconnect(messageWriter);
                messageWriter.Recycle();
            }
        }
        public static void SpawnGameDataMessage_SerializeValues_Postfix(SpawnGameDataMessage __instance, MessageWriter msg)
        {
            if (AmongUsClient.Instance.ClientId != __instance.ownerId && __instance.NetObjectType == Il2CppType.From(typeof(PlayerControl)))
            {
                FungleAPIPlugin.Instance.Log.LogInfo("Deserializing mods - Client");
                HandShakeManager.SendMods(msg);
            }
        }
        public static bool InnerNetClient_HandleGameDataInner_d__165_MoveNext_Prefix(InnerNetClient._HandleGameDataInner_d__165 __instance, ref bool __result)
        {
            InnerNetClient innerNetClient = __instance.__4__this;
            MessageReader reader = __instance.reader;
            if (__instance.__1__state != 0)
            {
                return true;
            }
            if (reader.Tag == 6)
            {
                int clientId = reader.ReadPackedInt32();
                ClientData clientData = innerNetClient.FindClientById(clientId);
                string sceneName = reader.ReadString();
                if (clientData != null && !string.IsNullOrWhiteSpace(sceneName))
                {
                    if (reader.BytesRemaining >= 0)
                    {
                        HandShakeManager.ReadModsAndCheck(reader, clientId);
                    }
                    else if (innerNetClient.AmHost)
                    {
                        innerNetClient.KickPlayer(clientId, false);
                        __result = false;
                        return false;
                    }
                    innerNetClient.StartCoroutine(innerNetClient.CoOnPlayerChangedScene(clientData, sceneName));
                }
                else
                {
                    UnityEngine.Debug.Log($"Couldn't find client {clientId} to change scene to {sceneName}");
                    reader.Recycle();
                }
                __result = false;
                return false;
            }
            if (reader.Tag == 200)
            {
                DisconnectReasons reason = (DisconnectReasons)reader.ReadByte();
                string extraText = reader.ReadString();
                string extraText2 = reader.ReadString();
                string text = DisconnectPopup.ErrorMessages[reason].GetString();
                if (!string.IsNullOrEmpty(extraText2) && !string.IsNullOrEmpty(extraText))
                {
                    text = text.Replace("0", extraText) + extraText2;
                }
                else if (!string.IsNullOrEmpty(extraText))
                {
                    text += extraText;
                }
                innerNetClient.Dispatcher.Add(new Action(delegate
                {
                    innerNetClient.HandleDisconnect(DisconnectReasons.Custom, text);
                    innerNetClient.LastCustomDisconnect = text;
                }));
                __result = false;
                return false;
            }
            return true;
        }
        public static void InnerNetClient_CoHandleSpawn_d__166_MoveNext_Postfix(InnerNetClient._CoHandleSpawn_d__166 __instance, bool __result)
        {
            if (ReactorSupport.ReactorAssembly == null && !__result && !AmongUsClient.Instance.AmHost && __instance._ownerId_5__2 == AmongUsClient.Instance.ClientId)
            {
                Hazel.MessageReader reader = __instance.reader;
                if (reader.BytesRemaining > 0)
                {
                    HandShakeManager.ReadModsAndCheck(reader, __instance._ownerId_5__2);
                    return;
                }
                AmongUsClient.Instance.HandleDisconnect(HandShakeManager.HostIsNotModded);
            }
        }
        public static bool InnerNetClient_CoSendSceneChange_d__156_MoveNext_Prefix(InnerNetClient._CoSendSceneChange_d__156 __instance, ref bool __result)
        {
            InnerNetClient innerNetClient = __instance.__4__this;
            if (!innerNetClient.AmHost && innerNetClient.connection.State == Hazel.ConnectionState.Connected && innerNetClient.ClientId >= 0)
            {
                ClientData clientData = innerNetClient.FindClientById(innerNetClient.ClientId);
                if (clientData != null)
                {
                    MessageWriter writer = MessageWriter.Get(SendOption.Reliable);
                    writer.StartMessage(Tags.GameData);
                    writer.Write(innerNetClient.GameId);
                    writer.StartMessage(6);
                    writer.WritePacked(innerNetClient.ClientId);
                    writer.Write(__instance.sceneName);
                    HandShakeManager.SendMods(writer);
                    writer.EndMessage();
                    writer.EndMessage();
                    innerNetClient.SendOrDisconnect(writer);
                    writer.Recycle();
                    innerNetClient.StartCoroutine(innerNetClient.CoOnPlayerChangedScene(clientData, __instance.sceneName));
                    __instance.__1__state = -1;
                    __result = false;
                    return false;
                }
            }
            return true;
        }
    }
}
