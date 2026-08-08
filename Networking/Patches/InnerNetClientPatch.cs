using BepInEx.Core.Logging.Interpolation;
using FungleAPI.Api;
using FungleAPI.ModCompatibility.ReactorSupportTemp;
using FungleAPI.PluginLoading;
using HarmonyLib;
using Hazel;
using Il2CppInterop.Generator.Extensions;
using Il2CppInterop.Runtime.InteropTypes.Arrays;
using InnerNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FungleAPI.Networking.Patches
{
    [HarmonyPatch(typeof(InnerNetClient))]
    internal static class InnerNetClientPatch
    {
        [HarmonyPatch(nameof(InnerNetClient.HandleMessage))]
        [HarmonyPrefix]
        public static bool Prefix(InnerNetClient __instance, [HarmonyArgument(0)] MessageReader reader)
        {
            if (__instance.NetworkMode == NetworkModes.FreePlay || ReactorCompatibility.Instance != null)
            {
                return true;
            }
            bool isFirst = false;
            if (!HandShakeManager.ModdedServerHandshakeActive.GetValueOrDefault())
            {
                Il2CppStructArray<byte> parentBuffer = reader.Parent.Buffer;
                if (parentBuffer[0] == 1)
                {
                    isFirst = (ushort)(((int)parentBuffer[1] << 8) + (int)parentBuffer[2]) == 1;
                }
            }
            if (reader.Tag == 255)
            {
                if (reader.ReadByte() == 0)
                {
                    reader.ReadString();
                    reader.ReadString();
                    reader.ReadPackedInt32();
                    if (isFirst)
                    {
                        HandShakeManager.ModdedServerHandshakeActive = true;
                    }
                }
                return false;
            }
            if (isFirst)
            {
                HandShakeManager.ModdedServerHandshakeActive = false;
            }
            return true;
        }
        [HarmonyPatch(nameof(InnerNetClient.GetConnectionData))]
        [HarmonyPostfix]
        public static void GetConnectionDataPostfix(ref Il2CppStructArray<byte> __result)
        {
            if (ReactorCompatibility.Instance != null) return;

            MessageWriter handshake = new MessageWriter(1000);
            handshake.Write(__result);
            handshake.Write(8243101772754678272UL | 2UL);

            handshake.WritePacked(HandShakeManager.RequiredMods.Count);
            foreach (BepInMod bepInMod in HandShakeManager.RequiredMods.Values)
            {
                handshake.Write(bepInMod.GUID);
                handshake.Write(bepInMod.Version);
                handshake.Write((ushort)1);
                handshake.Write(bepInMod.Name);
            }

            __result = handshake.ToByteArray(true);
            handshake.Recycle();
        }
        [HarmonyPatch(nameof(InnerNetClient.DisconnectInternal))]
        [HarmonyPrefix]
        public static void DisconnectInternalPrefix(InnerNetClient __instance, ref DisconnectReasons reason)
        {
            HandShakeManager.ModdedServerHandshakeActive = null;
            if (reason == DisconnectReasons.Kicked && (!string.IsNullOrEmpty(HandShakeManager.MissingMods) || !string.IsNullOrEmpty(HandShakeManager.ExtraMods) || !string.IsNullOrEmpty(AntiCheatManager.LastKickReason)))
            {
                if (!string.IsNullOrEmpty(AntiCheatManager.LastKickReason))
                {
                    reason = DisconnectReasons.Custom;
                    __instance.LastCustomDisconnect = AntiCheatManager.LastKickReason;
                }
                else
                {
                    StringBuilder stringBuilder = new StringBuilder();

                    if (!string.IsNullOrEmpty(HandShakeManager.MissingMods))
                    {
                        stringBuilder.Append(string.Format(FungleTranslation.HandShakeFail_MissingMods.GetString(), HandShakeManager.MissingMods));

                        if (!string.IsNullOrEmpty(HandShakeManager.ExtraMods))
                        {
                            stringBuilder.AppendLine();
                        }
                    }

                    if (!string.IsNullOrEmpty(HandShakeManager.ExtraMods))
                    {
                        stringBuilder.Append(string.Format(FungleTranslation.HandShakeFail_ExtraMods.GetString(), HandShakeManager.ExtraMods));
                    }

                    reason = DisconnectReasons.Custom;
                    __instance.LastCustomDisconnect = stringBuilder.ToString();
                }

                AntiCheatManager.LastKickReason = null;
                HandShakeManager.MissingMods = null;
                HandShakeManager.ExtraMods = null;
            }
        }
    }
}