using FungleAPI.Attributes;
using FungleAPI.PluginLoading;
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
    [FungleIgnore]
    public class CustomGameOver
    {
        public static CustomGameOver CachedGameOver;
        public List<CachedPlayerData> Winners = new List<CachedPlayerData>();
        public int GameOverId;
        public virtual string WinText { get; }
        public virtual Color BackgroundColor { get; }
        public virtual Color NameColor { get; }
        public virtual GameOverReason Reason { get; } = GameOverManager.GetValidGameOver();
        public virtual AudioClip Clip { get; }
        public virtual void SetData()
        {
            Winners.Clear();
            foreach (NetworkedPlayerInfo networkedPlayerInfo in GameData.Instance.AllPlayers)
            {
                if (networkedPlayerInfo.Role.DidWin(Reason))
                {
                    Winners.Add(new CachedPlayerData(networkedPlayerInfo));
                }
            }
        }
        public virtual void Serialize(MessageWriter messageWriter) { }
        public virtual void Deserialize(MessageReader messageReader) { }
        public virtual void SerializeRequest(MessageWriter messageWriter, PlayerControl owner) { Serialize(messageWriter); }
        public virtual void DeserializeRequest(MessageReader messageReader, PlayerControl owner) { Deserialize(messageReader); }
        public virtual void OnSetEverythingUp(EndGameManager endGameManager) { }
    }
}
