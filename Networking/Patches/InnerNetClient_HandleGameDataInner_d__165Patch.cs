using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using AmongUs.Data;
using AmongUs.InnerNet.GameDataMessages;
using BepInEx.Unity.IL2CPP.Utils.Collections;
using HarmonyLib;
using Hazel;
using InnerNet;
using static FungleAPI.PluginLoading.ModPlugin;

namespace FungleAPI.Networking.Patches
{
    [HarmonyPatch(typeof(InnerNetClient._HandleGameDataInner_d__165), "MoveNext")]
    internal static class InnerNetClient_HandleGameDataInner_d__165Patch
    {
        public static bool Prefix(InnerNetClient._HandleGameDataInner_d__165 __instance, ref bool __result)
        {
            InnerNetClient innerNetClient = __instance.__4__this;
            MessageReader reader = __instance.reader;
            if (__instance.__1__state != 0) return true;
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
                string text = HandShakeManager.ErrorMessages[reason].GetString();
                if (!string.IsNullOrEmpty(extraText2) && !string.IsNullOrEmpty(extraText))
                {
                    text = text.Replace("0", extraText) + extraText2;
                }
                else if (!string.IsNullOrEmpty(extraText))
                {
                    text += extraText2;
                }
                innerNetClient.EnqueueDisconnect(DisconnectReasons.Custom, text);
                __result = false;
                return false;
            }
            return true;
        }
    }
}
