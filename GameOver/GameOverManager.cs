using AmongUs.Data;
using FungleAPI.Event;
using FungleAPI.Event.Vanilla;
using FungleAPI.GameOver.Ends;
using FungleAPI.Networking;
using FungleAPI.Player.Networking;
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
    /// <summary>
    /// 
    /// </summary>
    [HarmonyPatch(typeof(EndGameResult))]
    public static class GameOverManager
    {
        public static bool AllowNonHostGameOverRequest = true;
        public static Dictionary<Type ,CustomGameOver> CustomGameOvers = new Dictionary<Type, CustomGameOver>();
        internal static int gameOverId = Enum.GetNames<GameOverReason>().Length;
        public static GameOverReason GetValidGameOver()
        {
            gameOverId++;
            return (GameOverReason)gameOverId;
        }
        /// <summary>
        /// Returns the instance of the given type
        /// </summary>
        public static T GetGameOverInstance<T>() where T : CustomGameOver
        {
            return GetGameOverInstance(typeof(T)).SimpleCast<T>() ?? null;
        }
        /// <summary>
        /// Returns the instance of the given type
        /// </summary>
        public static CustomGameOver GetGameOverInstance(Type type)
        {
            if (CustomGameOvers.TryGetValue(type, out CustomGameOver customGameOver))
            {
                return customGameOver;
            }
            return null;
        }
        /// <summary>
        /// Returns the instance of the given GameOverReason
        /// </summary>
        public static CustomGameOver GetGameOver(this GameOverReason gameOverReason)
        {
            return CustomGameOvers.Values.FirstOrDefault(g => g.Reason == gameOverReason);
        }
        /// <summary>
        /// Returns the instance of the given id
        /// </summary>
        public static CustomGameOver GetGameOverById(int Id)
        {
            return CustomGameOvers.Values.FirstOrDefault(g => g.GameOverId == Id);
        }
        public static void RpcEndGame<T>(this GameManager gameManager) where T : CustomGameOver
        {
            RpcEndGame(gameManager, GetGameOverInstance<T>());
        }
        public static void RpcEndGame(this GameManager gameManager, CustomGameOver gameOver)
        {
            AmongUsClient amongUsClient = AmongUsClient.Instance;
            if (!amongUsClient.AmHost && AllowNonHostGameOverRequest)
            {
                Rpc<RpcRequestGameOver>.Instance.Send(gameOver, PlayerControl.LocalPlayer, SendOption.Reliable, amongUsClient.HostId);
                return;
            }
            if (EventManager.CallEvent(new BeforeGameOver(gameOver)).Cancelled)
            {
                return;
            }
            gameManager.ShouldCheckForGameEnd = false;
            gameManager.logger.Info(string.Format("Endgame for {0}", gameOver.Reason), null);
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
            EventManager.CallEvent(new AfterGameOver(CustomGameOver.CachedGameOver));
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
                    __result = new EndGameResult(CustomGameOver.CachedGameOver.Reason, false, xpGrantResult, podsGrantResult, beansGrantResult);
                    return false;
                }
            }
            __result = new EndGameResult(CustomGameOver.CachedGameOver.Reason, false, null, null, null);
            return false;
        }
    }
}
