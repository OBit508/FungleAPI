using FungleAPI.Api;
using FungleAPI.Assets;
using FungleAPI.Extensions;
using FungleAPI.Utilities;
using HarmonyLib;
using Innersloth.IO;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace FungleAPI.GlobalPatches
{
    [HarmonyPatch(typeof(ServerManager))]
    internal static class ServerManagerPatch
    {
        [HarmonyPatch(nameof(ServerManager.Awake))]
        [HarmonyPostfix]
        public static void AwakePostfix(ServerManager __instance)
        {
            ServerManager.DefaultRegions = JsonConvert.DeserializeObject<ServerManager.JsonServerData>(AssetLoader.ReadText(FungleApiPlugin.Plugin.ModAssembly, "FungleAPI.Assets.FungleAssets.regionInfo.json"), new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.Auto
            }).Regions;
            __instance.serverInfoFileJson = Path.Combine(PlatformPaths.persistentDataPath, "customRegionInfo.json");
        }
        [HarmonyPatch(nameof(ServerManager.LoadServers))]
        [HarmonyPrefix]
        public static bool LoadServersPrefix(ServerManager __instance)
        {
            Debug.Log("ServerManager::LoadServers");
            if (FileIO.Exists(__instance.serverInfoFileJson))
            {
                try
                {
                    ServerManager.JsonServerData jsonServerData = JsonConvert.DeserializeObject<ServerManager.JsonServerData>(FileIO.ReadAllText(__instance.serverInfoFileJson), new JsonSerializerSettings
                    {
                        TypeNameHandling = TypeNameHandling.Auto
                    });
                    jsonServerData.CleanAndMerge(ServerManager.DefaultRegions);
                    __instance.AvailableRegions = jsonServerData.Regions.Where(r => !r.PingServer.EndsWith("among.us")).ToArray();
                    __instance.CurrentRegion = __instance.AvailableRegions[jsonServerData.CurrentRegionIdx.Wrap(__instance.AvailableRegions.Length)];
                    __instance.CurrentUdpServer = __instance.CurrentRegion.Servers.Random<ServerInfo>();
                    __instance.state = UpdateState.Success;
                    __instance.SaveServers();
                    return false;
                }
                catch (Exception ex)
                {
                    Debug.Log(string.Format("Couldn't load regions: {0}", ex));
                    __instance.StartCoroutine(__instance.ReselectRegionFromDefaults());
                    return false;
                }
            }
            __instance.StartCoroutine(__instance.ReselectRegionFromDefaults());
            return false;
        }
    }
}
