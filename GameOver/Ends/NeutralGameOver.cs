using FungleAPI.Translation;
using FungleAPI.Utilities;
using Hazel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace FungleAPI.GameOver.Ends
{
    public class NeutralGameOver : CustomGameOver
    {
        
        public override GameOverReason Reason { get; } = GameOverManager.GetValidGameOver();
        public override string WinText => FungleTranslation.neutralGameOver.GetString();
        public override Color BackgroundColor => Color.gray;
        public override Color NameColor => Color.gray;
        public override List<NetworkedPlayerInfo> GetWinners()
        {
            List<NetworkedPlayerInfo> winners = new List<NetworkedPlayerInfo>();
            foreach (NetworkedPlayerInfo networkedPlayerInfo in GameData.Instance.AllPlayers)
            {
                if (networkedPlayerInfo.Role.DidWin(Reason) && !networkedPlayerInfo.IsDead)
                {
                    winners.Add(networkedPlayerInfo);
                }
            }
            return winners;
        }
    }
}
