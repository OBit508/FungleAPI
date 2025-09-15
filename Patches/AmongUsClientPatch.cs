using AmongUs.Data;
using Assets.CoreScripts;
using BepInEx.Unity.IL2CPP.Utils.Collections;
using FungleAPI.Components;
using FungleAPI.Networking;
using FungleAPI.Networking.RPCs;
using FungleAPI.Role;
using FungleAPI.Translation;
using FungleAPI.Utilities;
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
        [HarmonyPatch("OnGameEnd")]
        [HarmonyPrefix]
        public static bool Prefix(AmongUsClient __instance, [HarmonyArgument(0)] EndGameResult endGameResult)
        {
            DataManager.Player.Ban.BanPoints -= 1.5f;
            DataManager.Player.Ban.PreviousGameStartDate = Il2CppSystem.DateTime.MinValue;
            GameOverReason gameOverReason = endGameResult.GameOverReason;
            bool showAd = endGameResult.ShowAd;
            List<IDisconnectHandler> disconnectHandlers = __instance.DisconnectHandlers.ToSystemList();
            disconnectHandlers.RemoveAll((IDisconnectHandler handler) => !handler.IsPersistent);
            __instance.DisconnectHandlers = disconnectHandlers.ToIl2CppList();
            if (Minigame.Instance)
            {
                try
                {
                    Minigame.Instance.Close();
                    Minigame.Instance.Close();
                }
                catch (Exception ex)
                {
                    __instance.logger.Error("AmongUsClient::OnGameEnd Exception: 1", null);
                    FungleAPIPlugin.Instance.Log.LogError(ex);
                }
            }
            float num = Time.realtimeSinceStartup - GameData.TimeGameStarted;
            DestroyableSingleton<DebugAnalytics>.Instance.Analytics.EndGame(num, gameOverReason, GameData.Instance.AllPlayers);
            try
            {
                DestroyableSingleton<UnityTelemetry>.Instance.EndGame(gameOverReason);
            }
            catch (Exception ex2)
            {
                __instance.logger.Error("AmongUsClient::OnGameEnd Exception: 2", null);
                FungleAPIPlugin.Instance.Log.LogError(ex2);
            }
            GameData.OnGameEnd();
            EndGameResult.CachedGameOverReason = gameOverReason;
            EndGameResult.CachedShowAd = showAd;
            GameManager.Instance.DidHumansWin(gameOverReason);
            ProgressionManager.XpGrantResult xpGrantResult = endGameResult.XpGrantResult ?? ProgressionManager.XpGrantResult.Default();
            ProgressionManager.CurrencyGrantResult currencyGrantResult = endGameResult.BeansGrantResult ?? ProgressionManager.CurrencyGrantResult.Default();
            ProgressionManager.CurrencyGrantResult currencyGrantResult2 = endGameResult.PodsGrantResult ?? ProgressionManager.CurrencyGrantResult.Default();
            EndGameResult.CachedXpGrantResult = xpGrantResult;
            EndGameResult.CachedBeansGrantResult = currencyGrantResult;
            EndGameResult.CachedPodsGrantResult = currencyGrantResult2;
            if (endGameResult.XpGrantResult != null)
            {
                DataManager.Player.Stats.Xp = endGameResult.XpGrantResult.OldXpAmount + endGameResult.XpGrantResult.GrantedXp;
                if (endGameResult.XpGrantResult.LevelledUp)
                {
                    DataManager.Player.Stats.Xp = endGameResult.XpGrantResult.OldXpAmount + endGameResult.XpGrantResult.GrantedXp - endGameResult.XpGrantResult.XpRequiredToLevelUp;
                    DataManager.Player.Stats.Level = endGameResult.XpGrantResult.NewLevel;
                    DataManager.Player.Stats.XpForNextLevel = endGameResult.XpGrantResult.XpRequiredToLevelUpNextLevel;
                }
                else
                {
                    DataManager.Player.Stats.Xp = endGameResult.XpGrantResult.OldXpAmount + endGameResult.XpGrantResult.GrantedXp;
                    DataManager.Player.Stats.Level = endGameResult.XpGrantResult.OldLevel;
                    DataManager.Player.Stats.XpForNextLevel = endGameResult.XpGrantResult.XpRequiredToLevelUp;
                }
            }
            DataManager.Player.Save();
            DestroyableSingleton<InventoryManager>.Instance.ChangePodCount(currencyGrantResult2.PodId, (int)currencyGrantResult2.GrantedPodsWithMultiplierApplied);
            DestroyableSingleton<InventoryManager>.Instance.UnusedBeans += (int)currencyGrantResult.GrantedPodsWithMultiplierApplied;
            List<NetworkedPlayerInfo> winners;
            if (EndGameHelper.Winners.TryGetValue(endGameResult, out winners))
            {
                foreach (NetworkedPlayerInfo networkedPlayerInfo in winners)
                {
                    EndGameResult.CachedWinners.Add(new CachedPlayerData(networkedPlayerInfo));
                }
            }
            else
            {
                for (int i = 0; i < GameData.Instance.PlayerCount; i++)
                {
                    NetworkedPlayerInfo networkedPlayerInfo = GameData.Instance.AllPlayers[i];
                    if (!(networkedPlayerInfo == null) && networkedPlayerInfo.Role.DidWin(gameOverReason))
                    {
                        EndGameResult.CachedWinners.Add(new CachedPlayerData(networkedPlayerInfo));
                    }
                }
            }
            EndGameResult.CachedLocalPlayer = new CachedPlayerData(PlayerControl.LocalPlayer.Data);
            GameDebugCommands.RemoveCommands();
            __instance.StartCoroutine(__instance.CoEndGame());
            return false;
        }
        [HarmonyPatch("Awake")]
        [HarmonyPostfix]
        public static void AwakePostfix(AmongUsClient __instance)
        {
            bool flag = false;
            foreach (ModPlugin plugin in ModPlugin.AllPlugins)
            {
                plugin.Settings.Initialize();
                if (plugin.UseShipReference)
                {
                    flag = true;
                }
            }
            if (flag)
            {
                __instance.StartCoroutine(Utilities.Prefabs.PrefabUtils.CoLoadShipPrefabs().WrapToIl2Cpp());
            }
        }
        [HarmonyPatch("CreatePlayer")]
        [HarmonyPostfix]
        public static void CreatePlayerPostfix(AmongUsClient __instance, [HarmonyArgument(0)] ClientData clientData)
        {
            if (!__instance.AmHost || clientData.Id == __instance.HostId)
            {
                return;
            }
            LobbyWarningText.nonModdedPlayers.Add(clientData, new Utilities.ChangeableValue<float>(5));
            CustomRpcManager.Instance<RpcSyncAllConfigs>().Send(null, PlayerControl.LocalPlayer.NetId, Hazel.SendOption.Reliable, clientData.Id);
        }
        public static DisconnectReasons FailedToSyncOptionsError = (DisconnectReasons)(-100);
    }
}
