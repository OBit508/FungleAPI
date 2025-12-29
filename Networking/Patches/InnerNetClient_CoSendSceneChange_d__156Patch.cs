using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;
using Hazel;
using InnerNet;

namespace FungleAPI.Networking.Patches
{
    [HarmonyPatch(typeof(InnerNetClient._CoSendSceneChange_d__156), "MoveNext")]
    internal static class InnerNetClient_CoSendSceneChange_d__156Patch
    {
        public static bool Prefix(InnerNetClient._CoSendSceneChange_d__156 __instance, ref bool __result)
        {
            InnerNetClient innerNetClient = __instance.__4__this;
            if (!innerNetClient.AmHost && innerNetClient.connection.State == Hazel.ConnectionState.Connected && innerNetClient.ClientId >= 0)
            {
                ClientData clientData = innerNetClient.FindClientById(innerNetClient.ClientId);
                if (clientData != null)
                {
                    MessageWriter writer = MessageWriter.Get(Hazel.SendOption.Reliable);
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
