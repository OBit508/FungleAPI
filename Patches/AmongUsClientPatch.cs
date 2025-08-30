using BepInEx.Unity.IL2CPP.Utils.Collections;
using FungleAPI.MCIPatches;
using FungleAPI.Networking.RPCs;
using FungleAPI.Networking;
using HarmonyLib;
using InnerNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace FungleAPI.Patches
{
    [HarmonyPatch(typeof(AmongUsClient))]
    internal static class AmongUsClientPatch
    {
        public static List<ClientData> NonModdedClients = new List<ClientData>();
        [HarmonyPatch("CreatePlayer")]
        [HarmonyPrefix]
        public static void CreatePlayerPrefix(AmongUsClient __instance, [HarmonyArgument(0)] ClientData clientData)
        {
            NonModdedClients.Add(clientData);
            if (clientData.Id != __instance.HostId)
            {
                if (__instance.AmHost)
                {
                    __instance.StartCoroutine(TryCheckClient(clientData).WrapToIl2Cpp());
                }
                else
                {
                    CustomRpcManager.Instance<RpcAmModdedClient>().CustomSend(clientData.Id);
                }
            }
        }
        [HarmonyPatch("KickNotJoinedPlayers")]
        [HarmonyPrefix]
        public static bool KickNotJoinedPlayersPrefix(AmongUsClient __instance)
        {
            if (__instance.AmHost)
            {
                Il2CppSystem.Collections.Generic.List<ClientData> allClients = __instance.allClients;
                lock (allClients)
                {
                    for (int i = 0; i < __instance.allClients.Count; i++)
                    {
                        ClientData clientData = __instance.allClients[i];
                        if (clientData.Character == null || NonModdedClients.Contains(clientData))
                        {
                            __instance.SendLateRejection(clientData.Id, DisconnectReasons.Error);
                            NonModdedClients.Remove(clientData);
                        }
                    }
                }
            }
            return false;
        }
        public static System.Collections.IEnumerator TryCheckClient(ClientData clientData)
        {
            while (clientData.IsBeingCreated)
            {
                yield return new WaitForSeconds(0.05f);
            }
            int count = 0;
            while (NonModdedClients.Contains(clientData))
            {
                if (MCIUtils.GetClient(clientData.Id) != null)
                {
                    FungleAPIPlugin.Plugin.BasePlugin.Log.LogInfo("MCI Bot detected");
                    NonModdedClients.Remove(clientData);
                    yield break;
                }
                else
                {
                    if (count == 10 || !AmongUsClient.Instance.allClients.Contains(clientData))
                    {
                        FungleAPIPlugin.Plugin.BasePlugin.Log.LogError("Failed to get FungleAPI on Client kicking");
                        if (AmongUsClient.Instance.allClients.Contains(clientData))
                        {
                            AmongUsClient.Instance.SendLateRejection(clientData.Id, DisconnectReasons.Kicked);
                        }
                    }
                    else
                    {
                        FungleAPIPlugin.Plugin.BasePlugin.Log.LogInfo("Trying to get FungleAPI on Client times: " + count.ToString());
                        yield return new WaitForSeconds(0.1f);
                        count++;
                    }
                }
            }
        }
    }
}
