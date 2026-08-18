using BepInEx.Core.Logging.Interpolation;
using FungleAPI.Api;
using FungleAPI.ModCompatibility;
using FungleAPI.ModCompatibility.ReactorSupportTemp;
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
using static Il2CppSystem.Net.WebSockets.ManagedWebSocket;

namespace FungleAPI.Networking.Patches
{
    [HarmonyPatch(typeof(InnerNetClient._CoHandleSpawn_d__169), "MoveNext")]
    internal static class CoHandleSpawnPatch
    {
        public const ulong Magic = 32199616299822962UL;
        public const byte Version = 2;
        public const int HeaderSize = 8;
        public static void Postfix(InnerNetClient._CoHandleSpawn_d__169 __instance, bool __result)
        {
            if (ReactorCompatibility.Instance != null || __result || HandShakeManager.ModdedServerHandshakeActive.GetValueOrDefault()) return;

            if (!AmongUsClient.Instance.AmHost && __instance._ownerId_5__2 == AmongUsClient.Instance.ClientId)
            {
                MessageReader messageReader = __instance.reader;
                if (messageReader.BytesRemaining > 0)
                {
                    try
                    {
                        if (IsReactor(messageReader))
                        {
                            HandShakeManager.DisconnectWithReason(FungleTranslation.HandShakeFail_HostUsingReactor.GetString());
                            return;
                        }

                        string str = messageReader.ReadString();
                        if (str != "FClient")
                        {
                            HandShakeManager.DisconnectWithReason(FungleTranslation.HandShakeFail_HostNotModded.GetString());
                        }
                    }
                    catch (Exception ex)
                    {
                        HandShakeManager.DisconnectWithReason(FungleTranslation.HandShakeFail_HostNotModded.GetString());
                        FunglePlugin<FungleApiPlugin>.Logger.LogError($"Disconnecting as HostNotModded Exception: {ex.Message}");
                    }
                    return;
                }
                HandShakeManager.DisconnectWithReason(FungleTranslation.HandShakeFail_HostNotModded.GetString());
            }
        }
        public static bool IsReactor(MessageReader reader)
        {
            if (reader.BytesRemaining < HeaderSize)
            {
                return false;
            }

            int position = reader.Position;
            try
            {
                ulong value = reader.ReadUInt64();

                ulong magic = value >> 8;
                byte version = (byte)(value & 0xFF);

                return magic == Magic && version == Version;
            }
            finally
            {
                reader.Position = position;
            }
        }
    }
}
