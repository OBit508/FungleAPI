using AmongUs.GameOptions;
using FungleAPI.PluginLoading;
using HarmonyLib;
using Hazel;
using InnerNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace FungleAPI.Networking
{
    [HarmonyPatch(typeof(InnerNetClient), "HostGame")]
    public static class HostGameHelper
    {
        public const byte HostModdedGame = 25;
        public static List<ModPlugin.Mod> Mods = new List<ModPlugin.Mod>();
        private static bool Prefix(InnerNetClient __instance, IGameOptions settings, GameFilterOptions filterOpts)
        {
            __instance.IsGamePublic = false;
            MessageWriter msg = MessageWriter.Get(SendOption.Reliable);
            msg.StartMessage(HostModdedGame);
            msg.WriteBytesAndSize(__instance.gameOptionsFactory.ToBytes(settings, AprilFoolsMode.IsAprilFoolsModeToggledOn));
            msg.Write(CrossplayMode.GetCrossplayFlags());
            filterOpts.Serialize(msg);
            using (MD5 md5 = MD5.Create())
            {
                msg.Write(md5.ComputeHash(Encoding.UTF8.GetBytes(string.Join(",", Mods))).Take(16).ToArray());
            }
            msg.EndMessage();
            __instance.SendOrDisconnect(msg);
            msg.Recycle();
            Debug.Log("Client requesting new custom game.");
            return false;
        }
    }
}
