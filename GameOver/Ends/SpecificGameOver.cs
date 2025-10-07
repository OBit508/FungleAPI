using FungleAPI.Networking;
using Hazel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace FungleAPI.GameOver.Ends
{
    public class SpecificGameOver : CustomGameOver
    {
        public List<NetworkedPlayerInfo> winners = new List<NetworkedPlayerInfo>();
        public string winText;
        public Color backgroundColor;
        public Color nameColor;
        public override GameOverReason Reason { get; } = GameOverManager.GetValidGameOver();
        public override string WinText => winText;
        public override Color BackgroundColor => backgroundColor;
        public override Color NameColor => nameColor;
        public override List<NetworkedPlayerInfo> GetWinners()
        {
            return winners;
        }
        public override void Serialize(MessageWriter messageWriter)
        {
            base.Serialize(messageWriter);
            messageWriter.Write(winText);
            messageWriter.WriteColor(backgroundColor);
            messageWriter.WriteColor(nameColor);
        }
        public override void Deserialize(MessageReader messageReader)
        {
            base.Deserialize(messageReader);
            winText = messageReader.ReadString();
            backgroundColor = messageReader.ReadColor();
            nameColor = messageReader.ReadColor();
        }
    }
}
