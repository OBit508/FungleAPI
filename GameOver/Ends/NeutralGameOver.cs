using FungleAPI.Role;
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
        public string win;
        public Color? color;
        public override GameOverReason Reason { get; } = GameOverManager.GetValidGameOver();
        public override string WinText => win == null ? FungleTranslation.neutralGameOver.GetString() : win;
        public override Color BackgroundColor => color == null ? Color.gray : color.Value;
        public override Color NameColor => BackgroundColor;
        public override void SetData()
        {
            win = null;
            color = null;
            List<NetworkedPlayerInfo> winners = new List<NetworkedPlayerInfo>();
            foreach (NetworkedPlayerInfo networkedPlayerInfo in GameData.Instance.AllPlayers)
            {
                if (networkedPlayerInfo.Role.DidWin(Reason))
                {
                    winners.Add(networkedPlayerInfo);
                    Winners.Add(new CachedPlayerData(networkedPlayerInfo));
                }
            }
            if (winners.Count == 1)
            {
                NetworkedPlayerInfo winner = winners[0];
                ICustomRole customRole = winner.Role.CustomRole();
                if (customRole != null)
                {
                    win = customRole.NeutralWinText;
                    color = customRole.RoleColor;
                }
            }
        }
    }
}
