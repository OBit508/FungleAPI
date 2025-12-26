using AmongUs.Data;
using Assets.CoreScripts;
using BepInEx.Unity.IL2CPP.Utils;
using BepInEx.Unity.IL2CPP.Utils.Collections;
using FungleAPI.Components;
using FungleAPI.Networking;
using FungleAPI.Networking.RPCs;
using FungleAPI.PluginLoading;
using FungleAPI.Role;
using FungleAPI.Teams;
using FungleAPI.Translation;
using FungleAPI.Utilities;
using HarmonyLib;
using Hazel;
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
        [HarmonyPatch("Awake")]
        [HarmonyPostfix]
        public static void AwakePostfix(AmongUsClient __instance)
        {
            foreach (ModdedTeam team in ModdedTeam.Teams)
            {
                team.Initialize(ModPluginManager.GetModPlugin(team.GetType().Assembly));
            }
            foreach (ModPlugin plugin in ModPlugin.AllPlugins)
            {
                plugin.Settings.Initialize();
                plugin.Options.AddRange(plugin.Settings.Options);
            }
        }
        [HarmonyPatch("CreatePlayer")]
        [HarmonyPostfix]
        public static void CreatePlayerPostfix(AmongUsClient __instance, [HarmonyArgument(0)] ClientData clientData)
        {
            if (clientData.Id == __instance.HostId || !__instance.AmHost)
            {
                return;
            }
            LobbyWarningText.nonModdedPlayers.Add(clientData, (new ChangeableValue<float>(5), new ChangeableValue<float>(1.5f)));
            __instance.StartCoroutine(SafeSend(new Action(delegate
            {
                CustomRpcManager.Instance<RpcSyncAllConfigs>().Send(PlayerControl.LocalPlayer, SendOption.Reliable, clientData.Id);
            })));
        }
        public static System.Collections.IEnumerator SafeSend(Action ac)
        {
            while (PlayerControl.LocalPlayer == null || PlayerControl.LocalPlayer.Data == null || PlayerControl.LocalPlayer.Data.ClientId == -1 || AmongUsClient.Instance.ClientId == -1)
            {
                yield return null;
            }
            ac();
        }
        public static DisconnectReasons FailedToSyncOptionsError = (DisconnectReasons)(-100);
        public static DisconnectReasons MissingMods = (DisconnectReasons)(-101);
        public static DisconnectReasons MissingModsOnHost = (DisconnectReasons)(-102);
        public static DisconnectReasons NotTheSameMods = (DisconnectReasons)(-103);
    }
}
