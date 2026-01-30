using AmongUs.Data;
using FungleAPI.GameOver.Ends;
using FungleAPI.Networking;
using FungleAPI.Networking.RPCs;
using FungleAPI.PluginLoading;
using FungleAPI.Role;
using FungleAPI.Teams;
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

namespace FungleAPI.GameOver
{
    [HarmonyPatch(typeof(EndGameResult))]
    public static class GameOverManager
    {
        public static List<CustomGameOver> AllCustomGameOver = new List<CustomGameOver>();
        internal static int gameOverId = Enum.GetNames<GameOverReason>().Length;
        public static GameOverReason GetValidGameOver()
        {
            gameOverId++;
            return (GameOverReason)gameOverId;
        }
        public static T Instance<T>() where T : CustomGameOver
        {
            return AllCustomGameOver.FirstOrDefault(g => g.GetType() == typeof(T)).SimpleCast<T>();
        }
        public static CustomGameOver GetGameOver(this GameOverReason gameOverReason)
        {
            return AllCustomGameOver.FirstOrDefault(g => g.Reason == gameOverReason);
        }
        public static void RpcEndGame<T>(this GameManager gameManager) where T : CustomGameOver
        {
            RpcEndGame(gameManager, Instance<T>());
        }
        public static void RpcEndGame(this GameManager gameManager, CustomGameOver gameOver)
        {
            gameManager.ShouldCheckForGameEnd = false;
            gameManager.logger.Info(string.Format("Endgame for {0}", gameOver.Reason), null);
            AmongUsClient amongUsClient = AmongUsClient.Instance;
            MessageWriter messageWriter = amongUsClient.StartEndGame();
            messageWriter.WriteGameOver(gameOver);
            gameOver.Serialize(messageWriter);
            amongUsClient.FinishEndGame(messageWriter);
        }
        [HarmonyPatch("Create")]
        [HarmonyPrefix]
        private static bool CreatePrefix([HarmonyArgument(0)] MessageReader reader, ref EndGameResult __result)
        {
            CustomGameOver.CachedGameOver = reader.ReadGameOver();
            CustomGameOver.CachedGameOver.Deserialize(reader);
            CustomGameOver.CachedGameOver.SetData();
            bool showAd = reader.ReadBoolean();
            if (reader.Position < reader.Length)
            {
                MessageReader messageReader = reader.ReadMessage();
                if (messageReader.Tag == 1)
                {
                    uint oldXpAmount = (uint)messageReader.ReadInt32();
                    uint grantedXp = (uint)messageReader.ReadInt32();
                    uint num = (uint)messageReader.ReadInt32();
                    messageReader.ReadInt32();
                    uint oldLevel = (uint)messageReader.ReadInt32();
                    uint newLevel = (uint)messageReader.ReadInt32();
                    uint maxLevel = (uint)messageReader.ReadInt32();
                    string text = messageReader.ReadString();
                    messageReader.ReadInt32();
                    uint grantedPodsWithMultiplierApplied = (uint)messageReader.ReadInt32();
                    messageReader.ReadInt32();
                    uint grantedPodsWithMultiplierApplied2 = (uint)messageReader.ReadInt32();
                    bool flag = messageReader.ReadBoolean();
                    float multiplier = 1f;
                    float multiplier2 = 1f;
                    uint xpRequiredToLevelUpNextLevel = num;
                    if (flag)
                    {
                        multiplier = messageReader.ReadSingle();
                        multiplier2 = messageReader.ReadSingle();
                        xpRequiredToLevelUpNextLevel = (uint)messageReader.ReadInt32();
                    }
                    ProgressionManager.XpGrantResult xpGrantResult = new ProgressionManager.XpGrantResult(grantedXp, oldXpAmount, num, xpRequiredToLevelUpNextLevel, flag, oldLevel, newLevel, maxLevel);
                    ProgressionManager.CurrencyGrantResult podsGrantResult = new ProgressionManager.CurrencyGrantResult(text, (uint)DestroyableSingleton<InventoryManager>.Instance.GetPodCount(text), grantedPodsWithMultiplierApplied, multiplier);
                    ProgressionManager.CurrencyGrantResult beansGrantResult = new ProgressionManager.CurrencyGrantResult(Constants.Beans, (uint)DestroyableSingleton<InventoryManager>.Instance.UnusedBeans, grantedPodsWithMultiplierApplied2, multiplier2);
                    __result = new EndGameResult(CustomGameOver.CachedGameOver.Reason, showAd, xpGrantResult, podsGrantResult, beansGrantResult);
                    return false;
                }
            }
            __result = new EndGameResult(CustomGameOver.CachedGameOver.Reason, false, null, null, null);
            return false;
        }
    }
}
