using AmongUs.Data;
using FungleAPI.GameOver.Ends;
using FungleAPI.Networking;
using FungleAPI.Role;
using FungleAPI.Role.Teams;
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
        internal static CustomGameOver RegisterGameOver(Type type, ModPlugin plugin)
        {
            CustomGameOver gameOver = (CustomGameOver)Activator.CreateInstance(type);
            plugin.BasePlugin.Log.LogInfo("Registered GameOver " + type.Name + " Id: " + ((int)gameOver.Reason).ToString());
            AllCustomGameOver.Add(gameOver);
            return gameOver;
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
            AmongUsClient amongUsClient = AmongUsClient.Instance;
            MessageWriter messageWriter = amongUsClient.StartEndGame();
            messageWriter.WriteGameOver(gameOver);
            gameOver.Serialize(messageWriter);
            messageWriter.Write(false);
            amongUsClient.FinishEndGame(messageWriter);
        }
        public static void RpcEndGame(this GameManager gameManager, ModdedTeam team)
        {
            if (team.DefaultGameOver != null)
            {
                RpcEndGame(gameManager, team.DefaultGameOver);
                return;
            }
            List<NetworkedPlayerInfo> winners = new List<NetworkedPlayerInfo>();
            foreach (NetworkedPlayerInfo networkedPlayerInfo in GameData.Instance.AllPlayers)
            {
                if (networkedPlayerInfo.Role != null && networkedPlayerInfo.Role.GetTeam() == team)
                {
                    winners.Add(networkedPlayerInfo);
                }
            }
            RpcEndGame(gameManager, winners, team.VictoryText, team.TeamColor, team.TeamColor);
        }
        public static void RpcEndGame(this GameManager gameManager, List<NetworkedPlayerInfo> winners, string winText, Color backgroundColor, Color nameColor)
        {
            SpecificGameOver end = Instance<SpecificGameOver>();
            end.winners = winners;
            end.winText = winText;
            end.backgroundColor = backgroundColor;
            end.nameColor = nameColor;
            RpcEndGame(gameManager, end);
        }
        [HarmonyPatch("Create")]
        [HarmonyPrefix]
        private static bool CreatePrefix([HarmonyArgument(0)] MessageReader reader, ref EndGameResult __result)
        {
            CustomGameOver.CachedGameOver = reader.ReadGameOver();
            CustomGameOver.CachedGameOver.Deserialize(reader);
            reader.ReadBoolean();
            if (reader.Position < reader.Length)
            {
                MessageReader messageReader = reader.ReadMessage();
                if (messageReader.Tag == 1)
                {
                    uint num = (uint)messageReader.ReadInt32();
                    uint num2 = (uint)messageReader.ReadInt32();
                    uint num3 = (uint)messageReader.ReadInt32();
                    messageReader.ReadInt32();
                    uint num4 = (uint)messageReader.ReadInt32();
                    uint num5 = (uint)messageReader.ReadInt32();
                    uint num6 = (uint)messageReader.ReadInt32();
                    string text = messageReader.ReadString();
                    messageReader.ReadInt32();
                    uint num7 = (uint)messageReader.ReadInt32();
                    messageReader.ReadInt32();
                    uint num8 = (uint)messageReader.ReadInt32();
                    bool flag2 = messageReader.ReadBoolean();
                    float num9 = 1f;
                    float num10 = 1f;
                    uint num11 = num3;
                    if (flag2)
                    {
                        num9 = messageReader.ReadSingle();
                        num10 = messageReader.ReadSingle();
                        num11 = (uint)messageReader.ReadInt32();
                    }
                    ProgressionManager.XpGrantResult xpGrantResult = new ProgressionManager.XpGrantResult(num2, num, num3, num11, flag2, num4, num5, num6);
                    ProgressionManager.CurrencyGrantResult currencyGrantResult = new ProgressionManager.CurrencyGrantResult(text, (uint)DestroyableSingleton<InventoryManager>.Instance.GetPodCount(text), num7, num9);
                    ProgressionManager.CurrencyGrantResult currencyGrantResult2 = new ProgressionManager.CurrencyGrantResult(Constants.Beans, (uint)DestroyableSingleton<InventoryManager>.Instance.UnusedBeans, num8, num10);
                    __result = new EndGameResult(CustomGameOver.CachedGameOver.Reason, false, xpGrantResult, currencyGrantResult, currencyGrantResult2);
                    return false;
                }
            }
            __result = new EndGameResult(CustomGameOver.CachedGameOver.Reason, false, null, null, null);
            return false;
        }
    }
}
