using FungleAPI.ModCompatibility;
using FungleAPI.Player.Networking;
using FungleAPI.PluginLoading;
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
    [HarmonyPatch(typeof(InnerNetClient._CoSendSceneChange_d__156), nameof(InnerNetClient._CoSendSceneChange_d__156.MoveNext))]
    internal static class CoSendSceneChangePatch
    {
        public static bool Prefix(InnerNetClient._CoSendSceneChange_d__156 __instance, ref bool __result)
        {
            InnerNetClient innerNetClient = __instance.__4__this;
            if (!innerNetClient.AmHost && innerNetClient.connection.State == ConnectionState.Connected && innerNetClient.ClientId >= 0 && ReactorSupport.ReactorAssembly == null)
            {
                ClientData clientData = innerNetClient.FindClientById(innerNetClient.ClientId);
                if (clientData != null)
                {
                    MessageWriter messageWriter = MessageWriter.Get(SendOption.Reliable);
                    messageWriter.StartMessage(5);
                    messageWriter.Write(innerNetClient.GameId);
                    messageWriter.StartMessage(6);
                    messageWriter.WritePacked(innerNetClient.ClientId);
                    messageWriter.Write(__instance.sceneName);

                    messageWriter.WritePacked(HandShakeManager.RequiredMods.Count);

                    foreach (BepInMod bepInMod in HandShakeManager.RequiredMods.Values)
                    {
                        messageWriter.Write(bepInMod.GUID);
                        messageWriter.Write(bepInMod.Version);
                        messageWriter.Write(bepInMod.Name);
                    }

                    messageWriter.EndMessage();
                    messageWriter.EndMessage();
                    innerNetClient.SendOrDisconnect(messageWriter);
                    messageWriter.Recycle();
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
