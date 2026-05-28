using HarmonyLib;
using Hazel;
using InnerNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FungleAPI.Networking.Patches
{
    [HarmonyPatch(typeof(InnerNetClient), nameof(InnerNetClient.StartRpcImmediately))]
    internal static class InnerNetClientPatch
    {
        public static bool Prefix(InnerNetClient __instance, uint targetNetId, byte callId, SendOption option, int targetClientId, ref MessageWriter __result)
        {
            MessageWriter messageWriter = MessageWriter.Get(option);
            if (targetClientId < 0)
            {
                messageWriter.StartMessage(5);
                messageWriter.Write(__instance.GameId);
            }
            else
            {
                messageWriter.StartMessage(6);
                messageWriter.Write(__instance.GameId);
                messageWriter.WritePacked(targetClientId);
            }
            messageWriter.StartMessage(2);
            messageWriter.Write(false);
            messageWriter.WritePacked(targetNetId);
            messageWriter.Write(callId);
            __result = messageWriter;
            return false;
        }
    }
}
