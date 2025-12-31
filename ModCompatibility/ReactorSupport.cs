using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using AmongUs.InnerNet.GameDataMessages;
using BepInEx;
using BepInEx.Unity.IL2CPP;
using FungleAPI.Translation;
using HarmonyLib;
using Hazel;
using Il2CppInterop.Runtime.InteropTypes.Arrays;
using InnerNet;
using Rewired.Internal.Localization;

namespace FungleAPI.ModCompatibility
{
    public static class ReactorSupport
    {
        public static string ReactorCreditsText => ReactorCredits_GetText == null ? null : (string)ReactorCredits_GetText.Invoke(null, new object[] { ReactorCredits_Location_PingTracker });
        public static bool DisableServerAuthority => DisableServerAuthorityPatch_Enabled == null ? false : (bool)DisableServerAuthorityPatch_Enabled.GetValue(null);
        public static PropertyInfo DisableServerAuthorityPatch_Enabled;
        public static PropertyInfo ModList_Current;
        public static MethodInfo ReactorCredits_GetText;
        public static MethodInfo ModdedHandshakeC2S_Serialize;
        public static MethodInfo ModdedHandshakeC2S_Deserialize;
        public static MethodInfo Reactor_LocalizationManager_TryGetTextFormatted;
        public static object ReactorCredits_Location_PingTracker;
        public static Type ReactorConnection;
        public static Type Mod;
        public static Type ReactorClientData;
        public static Assembly ReactorAssembly;
        public static BasePlugin ReactorPlugin;
        public static Harmony ReactorHarmony;
        public static void Initialize()
        {
            if (IL2CPPChainloader.Instance.Plugins.TryGetValue("gg.reactor.api", out PluginInfo pluginInfo))
            {
                FungleAPIPlugin.Instance.Log.LogInfo("Initializing Reactor Support");
                ReactorAssembly = pluginInfo.Instance.GetType().Assembly;
                ReactorPlugin = (BasePlugin)pluginInfo.Instance;
                ReactorHarmony = (Harmony)pluginInfo.Instance.GetType().GetProperty("Harmony").GetValue(ReactorPlugin);
                foreach (Type type in ReactorAssembly.GetTypes())
                {
                    if (type.FullName == "Reactor.Utilities.ReactorCredits")
                    {
                        ReactorCredits_GetText = type.GetMethod("GetText", AccessTools.all);
                        ReactorCredits_Location_PingTracker = Enum.Parse(type.GetNestedType("Location"), "PingTracker");
                        type.GetMethod("Register", new Type[] { typeof(string), typeof(string), typeof(bool), type.GetField("AlwaysShow").FieldType }).Invoke(null, new object[] { "FungleAPI", FungleAPIPlugin.ModV, false, null });
                    }
                    else if (type.FullName == "Reactor.Patches.Miscellaneous.DisableServerAuthorityPatch")
                    {
                        DisableServerAuthorityPatch_Enabled = type.GetProperty("Enabled");
                        ReactorHarmony.Unpatch(typeof(Constants).GetMethod("GetBroadcastVersion"), type.GetMethod("GetBroadcastVersionPatch"));
                        FungleAPIPlugin.Instance.Log.LogWarning("Patched Reactor.Patches.Miscellaneous.DisableServerAuthorityPatch.GetBroadcastVersionPatch");
                        ReactorHarmony.Unpatch(typeof(Constants).GetMethod("IsVersionModded"), type.GetMethod("IsVersionModdedPatch"));
                        FungleAPIPlugin.Instance.Log.LogWarning("Patched Reactor.Patches.Miscellaneous.DisableServerAuthorityPatch.IsVersionModdedPatch");
                    }
                    else if (type.FullName == "Reactor.Patches.Miscellaneous.PingTrackerPatch")
                    {
                        ReactorHarmony.Unpatch(typeof(PingTracker).GetMethod("Update"), type.GetMethod("Postfix"));
                        FungleAPIPlugin.Instance.Log.LogWarning("Patched Reactor.Patches.Miscellaneous.PingTrackerPatch.Postfix");
                    }
                    else if (type.FullName == "Reactor.Networking.Mod")
                    {
                        Mod = type;
                    }
                    else if (type.FullName == "Reactor.Networking.Messages.ModdedHandshakeC2S")
                    {
                        ModdedHandshakeC2S_Serialize = type.GetMethod("Serialize");
                        ModdedHandshakeC2S_Deserialize = type.GetMethod("Deserialize");
                    }
                    else if (type.FullName == "Reactor.Networking.ModList")
                    {
                        ModList_Current = type.GetProperty("Current");
                    }
                    else if (type.FullName == "Reactor.Networking.Patches.ReactorClientData")
                    {
                        ReactorClientData = type;
                    }
                    else if (type.FullName == "Reactor.Networking.Patches.ClientPatches")
                    {
                        ReactorHarmony.Unpatch(typeof(InnerNetClient._CoHandleSpawn_d__166).GetMethod("MoveNext"), type.GetNestedType("CoHandleSpawnPatch").GetMethod("Postfix"));
                        FungleAPIPlugin.Instance.Log.LogWarning("Patched Reactor.Networking.Patches.ClientPatche.CoHandleSpawnPatch.Postfix");
                        ReactorHarmony.Unpatch(typeof(InnerNetClient._HandleGameDataInner_d__165).GetMethod("MoveNext"), type.GetNestedType("HandleGameDataInnerPatch").GetMethod("Prefix"));
                        FungleAPIPlugin.Instance.Log.LogWarning("Patched Reactor.Networking.Patches.ClientPatche.HandleGameDataInnerPatch.Prefix");
                        ReactorHarmony.Unpatch(typeof(InnerNetClient._CoSendSceneChange_d__156).GetMethod("MoveNext"), type.GetNestedType("CoSendSceneChangePatch").GetMethod("Prefix"));
                        FungleAPIPlugin.Instance.Log.LogWarning("Patched Reactor.Networking.Patches.ClientPatche.CoSendSceneChangePatch.Prefix");
                        ReactorHarmony.Unpatch(typeof(SpawnGameDataMessage).GetMethod("SerializeValues"), type.GetNestedType("SpawnGameDataMessagePatch").GetMethod("Prefix"));
                        FungleAPIPlugin.Instance.Log.LogWarning("Patched Reactor.Networking.Patches.ClientPatche.SpawnGameDataMessagePatch.Prefix");
                    }
                    else if (type.FullName == "Reactor.Networking.Patches.ReactorConnection")
                    {
                        ReactorConnection = type;
                    }
                    else if (type.FullName == "Reactor.Localization.Utilities.CustomStringName")
                    {
                        FungleAPIPlugin.Harmony.Patch(type.GetMethod("Create"), new HarmonyMethod(typeof(ReactorSupport).GetMethod("CustomStringName_Create_Prefix")));
                        FungleAPIPlugin.Instance.Log.LogWarning("Patched Reactor.Localization.Utilities.CustomStringName.Create");
                    }
                    else if (type.FullName == "Reactor.Localization.LocalizationManager")
                    {
                        Reactor_LocalizationManager_TryGetTextFormatted = type.GetMethod("TryGetTextFormatted", AccessTools.all);
                    }
                    else if (type.FullName == "Reactor.Localization.Patches.GetStringPatch")
                    {
                        ReactorHarmony.Unpatch(typeof(TranslationController).GetMethod("GetString", new Type[] { typeof(StringNames), typeof(Il2CppReferenceArray<Il2CppSystem.Object>) }), type.GetMethod("StringNamesPatch"));
                        ReactorHarmony.Unpatch(typeof(TranslationController).GetMethod("GetStringWithDefault", new Type[] { typeof(StringNames), typeof(string), typeof(Il2CppReferenceArray<Il2CppSystem.Object>) }), type.GetMethod("StringNamesPatch"));
                        FungleAPIPlugin.Instance.Log.LogWarning("Patched Reactor.Localization.Patches.GetStringPatch.StringNamesPatch");
                    }
                }                
            }
        }
        public static void SendHandShakeMessage(MessageWriter messageWriter)
        {
            if (ModdedHandshakeC2S_Serialize != null)
            {
                FungleAPIPlugin.Instance.Log.LogDebug("[Reactor] Injecting ReactorHandshakeC2S to CoSendSceneChange");
                ModdedHandshakeC2S_Serialize.Invoke(null, new object[] { messageWriter, ModList_Current.GetValue(null) });
            }
        }
        public static void ReadHandShakeMessage(MessageReader messageReader, ClientData clientData)
        {
            if (ReactorClientData != null)
            {
                object[] mods;
                object[] args = new object[] { messageReader, null };
                ModdedHandshakeC2S_Deserialize.Invoke(null, args);
                mods = (object[])args[1];
                ReactorClientData.GetMethod("Set", AccessTools.all).Invoke(null, new object[] { clientData.Id, Activator.CreateInstance(ReactorClientData, new object[] { clientData, mods }) });
                FungleAPIPlugin.Instance.Log.LogDebug("[Reactor] Received reactor handshake for " + clientData.PlayerName);
                if (AmongUsClient.Instance.AmHost)
                {
                    object[] args2 = new object[] { mods, ModList_Current.GetValue(null), null };
                    if ((bool)Mod.GetMethod("Validate", AccessTools.all).Invoke(null, args2))
                    {
                        AmongUsClient.Instance.KickWithReason(clientData.Id, (string)args2[2]);
                    }
                }
            }
        }
        public static void ReadSetKickReasonMessage(MessageReader messageReader)
        {
            if (ReactorConnection != null)
            {
                string reason = messageReader.ReadString();
                FungleAPIPlugin.Instance.Log.LogDebug("[Reactor] Received SetKickReason: " + reason);
                object Instance = ReactorConnection.GetProperty("Instance").GetValue(null);
                if (Instance != null)
                {
                    ReactorConnection.GetProperty("LastKickReason").SetValue(Instance, reason);
                }
            }
            messageReader.Recycle();
        }
        public static bool CustomStringName_Create_Prefix(ref StringNames __result)
        {
            __result = (StringNames)Translator.validId;
            Translator.validId++;
            return false;
        }
        public static bool LocalizationManager_TryGetTextFormatted(StringNames stringName, Il2CppReferenceArray<Il2CppSystem.Object> parts, out string text)
        {
            if (Reactor_LocalizationManager_TryGetTextFormatted != null)
            {
                object[] args = new object[] { stringName, parts, null };
                bool result = (bool)Reactor_LocalizationManager_TryGetTextFormatted.Invoke(null, args);
                text = (string)args[2];
                return result;
            }
            text = "";
            return false;
        }
        internal static void KickWithReason(this InnerNetClient innerNetClient, int targetClientId, string reason)
        {
            MessageWriter writer = MessageWriter.Get(SendOption.Reliable);
            writer.StartMessage(Tags.GameDataTo);
            writer.Write(innerNetClient.GameId);
            writer.WritePacked(targetClientId);
            writer.StartMessage(byte.MaxValue);
            writer.Write(0);
            writer.Write(reason);
            writer.EndMessage();
            writer.EndMessage();
            innerNetClient.SendOrDisconnect(writer);
            writer.Recycle();
            innerNetClient.KickPlayer(targetClientId, false);
        }
    }
}
