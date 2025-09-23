using AmongUs.Data;
using HarmonyLib;
using Hazel;
using InnerNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace FungleAPI.Networking
{
    [HarmonyPatch(typeof(EndGameResult))]
    public static class EndGameHelper
    {
        internal static GameOverReason CustomGameOverReason;
        internal static List<CachedPlayerData> Winners = new List<CachedPlayerData>();
        internal static Color NameColor;
        public static void RpcCustomEndGame(List<NetworkedPlayerInfo> winners, Color nameColor)
        {
            AmongUsClient amongUsClient = AmongUsClient.Instance;
            MessageWriter messageWriter = amongUsClient.StartEndGame();
            messageWriter.Write((int)CustomGameOverReason);
            messageWriter.WriteColor(nameColor);
            messageWriter.Write(winners.Count);
            for (int i = 0; i < winners.Count; i++)
            {
                messageWriter.WriteNetObject(winners[i]);
            }
            messageWriter.Write(false);
            amongUsClient.FinishEndGame(messageWriter);
        }
        [HarmonyPatch("Create")]
        [HarmonyPrefix]
        private static bool CreatePrefix([HarmonyArgument(0)] MessageReader reader, ref EndGameResult __result)
        {
            Winners.Clear();
            GameOverReason gameOverReason = (GameOverReason)reader.ReadInt32();
            if (gameOverReason == CustomGameOverReason)
            {
                NameColor = reader.ReadColor();
                int count = reader.ReadInt32();
                for (int i = 0; i < count; i++)
                {
                    Winners.Add(new CachedPlayerData(reader.ReadNetObject<NetworkedPlayerInfo>()));
                }
            }
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
                    __result = new EndGameResult(gameOverReason, false, xpGrantResult, currencyGrantResult, currencyGrantResult2);
                    return false;
                }
            }
            __result = new EndGameResult(gameOverReason, false, null, null, null);
            return false;
        }
    }
}
