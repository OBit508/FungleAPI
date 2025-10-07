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
    public class CustomGameOver
    {
        public static CustomGameOver CachedGameOver;
        public List<CachedPlayerData> Winners = new List<CachedPlayerData>();
        public virtual string WinText { get; }
        public virtual Color BackgroundColor { get; }
        public virtual Color NameColor { get; }
        public virtual GameOverReason Reason { get; }
        public virtual AudioClip Clip { get; }
        public virtual List<NetworkedPlayerInfo> GetWinners()
        {
            List<NetworkedPlayerInfo> winners = new List<NetworkedPlayerInfo>();
            foreach (NetworkedPlayerInfo networkedPlayerInfo in GameData.Instance.AllPlayers)
            {
                if (networkedPlayerInfo.Role.DidWin(Reason))
                {
                    winners.Add(networkedPlayerInfo);
                }
            }
            return winners;
        }
        public virtual void Serialize(MessageWriter messageWriter)
        {
            Winners.Clear();
            List<NetworkedPlayerInfo> winners = GetWinners();
            messageWriter.Write(winners.Count);
            foreach (NetworkedPlayerInfo winner in winners)
            {
                Winners.Add(new CachedPlayerData(winner));
                messageWriter.WriteNetObject(winner);
            }
        }
        public virtual void Deserialize(MessageReader messageReader)
        {
            Winners.Clear();
            int count = messageReader.ReadInt32();
            for (int i = 0; i < count; i++)
            {
                Winners.Add(new CachedPlayerData(messageReader.ReadNetObject<NetworkedPlayerInfo>()));
            }
        }
        public virtual void OnSetEverythingUp(EndGameManager endGameManager)
        {
        }
    }
}
