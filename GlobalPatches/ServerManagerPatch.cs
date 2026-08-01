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
    [HarmonyPatch(typeof(ServerManager), nameof(ServerManager.Awake))]
    internal static class ServerManagerPatch
    {
        public static bool Prefix(ServerManager __instance)
        {
            if (!ServerManager._instance)
            {
                ServerManager._instance = __instance;
                if (__instance.DontDestroy)
                {
                    GameObject.DontDestroyOnLoad(__instance.gameObject);
                }
            }
            else if (ServerManager._instance != __instance)
            {
                __instance.gameObject.Destroy();
            }

            if (DestroyableSingleton<ServerManager>.Instance != __instance)
            {
                return false;
            }

            ServerManager.DefaultRegions = JsonConvert.DeserializeObject<ServerManager.JsonServerData>(AssetLoader.ReadText(FungleApiPlugin.Plugin.ModAssembly, "FungleAPI.Assets.FungleAssets.regionInfo.json"), new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.Auto
            }).Regions;
            __instance.serverInfoFileJson = Path.Combine(PlatformPaths.persistentDataPath, "CustomRegionInfo.json");
            __instance.LoadServers();
            __instance.HandleUpnp();
            return false;
        }
    }
}
