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
    [HarmonyPatch(typeof(ServerManager), nameof(ServerManager.LoadServers))]
    internal static class ServerManagerPatch
    {
        public static void Prefix(ServerManager __instance)
        {
            ServerManager.DefaultRegions = JsonConvert.DeserializeObject<ServerManager.JsonServerData>(AssetLoader.ReadText(FungleApiPlugin.Plugin.ModAssembly, "FungleAPI.Assets.FungleAssets.regionInfo.json"), new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.Auto
            }).Regions;
            __instance.serverInfoFileJson = Path.Combine(PlatformPaths.persistentDataPath, "customRegionInfo.json");
        }
    }
}
