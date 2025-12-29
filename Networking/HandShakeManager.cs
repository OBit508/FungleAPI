using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AmongUs.Data;
using AmongUs.InnerNet.GameDataMessages;
using BepInEx.Core.Logging.Interpolation;
using BepInEx.Unity.IL2CPP.Utils.Collections;
using FungleAPI.Patches;
using FungleAPI.PluginLoading;
using FungleAPI.Translation;
using FungleAPI.Utilities;
using Hazel;
using Il2CppInterop.Runtime;
using InnerNet;
using Steamworks;
using UnityEngine;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace FungleAPI.Networking
{
    internal static class HandShakeManager
    {
        public static Dictionary<DisconnectReasons, Translator> ErrorMessages = new Dictionary<DisconnectReasons, Translator>();
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
                messageWriter.WriteMod(plugin.LocalMod);
            }
        }
        public static void ReadModsAndCheck(MessageReader messageReader, int clientId)
        {
            AmongUsClient amongUsClient = AmongUsClient.Instance;
            try
            {
                List<string> extraMods = new List<string>();
                List<string> missingMods = new List<string>();
                List<ModPlugin.Mod> mods = new List<ModPlugin.Mod>();
                int modCount = messageReader.ReadInt32();
                for (int i = 0; i < modCount; i++)
                {
                    ModPlugin.Mod mod = messageReader.ReadMod(out string str);
                    if (mod == null)
                    {
                        extraMods.Add(str);
                    }
                    else
                    {
                        mods.Add(mod);
                    }
                }
                foreach (ModPlugin plugin in ModPlugin.AllPlugins)
                {
                    if (!mods.Any(m => m.Equals(plugin.LocalMod)))
                    {
                        missingMods.Add(plugin.LocalMod.GUID);
                    }
                }
                if (amongUsClient.AmHost)
                {
                    if (missingMods.Count > 0 && extraMods.Count > 0)
                    {
                        amongUsClient.KickPlayer(clientId, NotSameMods, string.Join(", ", missingMods) + ".", string.Join(", ", extraMods) + ".");
                    }
                    else
                    {
                        if (missingMods.Count > 0)
                        {
                            amongUsClient.KickPlayer(clientId, MissingMods, string.Join(", ", missingMods) + ".");
                        }
                        if (extraMods.Count > 0)
                        {
                            amongUsClient.KickPlayer(clientId, MissingModsOnHost, string.Join(", ", extraMods) + ".");
                        }
                    }
                }
            }
            catch
            {
                if (amongUsClient.AmHost)
                {
                    amongUsClient.KickPlayer(clientId, FailedToVerifyMods);
                }
            }
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
    }
}
