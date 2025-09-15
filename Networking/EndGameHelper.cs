using HarmonyLib;
using Hazel;
using InnerNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Services.Core.Threading.Internal;

namespace FungleAPI.Networking
{
    [HarmonyPatch(typeof(EndGameResult), "Create")]
    public static class EndGameHelper
    {
        public static Dictionary<EndGameResult, List<NetworkedPlayerInfo>> Winners = new Dictionary<EndGameResult, List<NetworkedPlayerInfo>>();
        public static void RpcCustomEndGame(this GameManager gameManager, List<NetworkedPlayerInfo> winners, bool showAd)
        {
            gameManager.ShouldCheckForGameEnd = false;
            gameManager.logger.Info(string.Format("CustomEndGame for {0}", winners.Count.ToString()), null);
            MessageWriter messageWriter = AmongUsClient.Instance.StartEndGame();
            messageWriter.Write(true);
            messageWriter.Write(winners.Count);
            for (int i = 0; i < winners.Count; i++)
            {
                messageWriter.WriteNetObject(winners[i]);
            }
            messageWriter.Write(showAd);
            AmongUsClient.Instance.FinishEndGame(messageWriter);
        }
        private static bool Prefix([HarmonyArgument(0)] MessageReader reader, ref EndGameResult __result)
        {
            List<NetworkedPlayerInfo> winners = new List<NetworkedPlayerInfo>();
            bool CustomGameOver = reader.ReadBoolean();
            GameOverReason gameOverReason = (GameOverReason)(-1);
            if (!CustomGameOver)
            {
                gameOverReason = (GameOverReason)reader.ReadByte();
            }
            else
            {
                int count = reader.ReadInt32();
                for (int i = 0; i < count; i++)
                {
                    winners.Add(reader.ReadNetObject<NetworkedPlayerInfo>());
                }
            }
            bool flag = reader.ReadBoolean();
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
                    __result = new EndGameResult(gameOverReason, flag, xpGrantResult, currencyGrantResult, currencyGrantResult2);
                    if (CustomGameOver)
                    {
                        Winners.Add(__result, winners);
                    }
                    return false;
                }
            }
            __result = new EndGameResult(gameOverReason, flag, null, null, null);
            if (CustomGameOver)
            {
                Winners.Add(__result, winners);
            }
            return false;
        }
    }
}
