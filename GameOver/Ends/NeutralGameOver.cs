using FungleAPI.Player;
using FungleAPI.Role;
using FungleAPI.Role.Utilities;
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
    /// <summary>
    /// Used to give the victory to the last neutral assassin alive
    /// </summary>
    public class NeutralGameOver : BaseGameOver<PlayerControl>
    {
        public PlayerControl Winner;
        public override string WinText { get; set; }
        public override Color BackgroundColor { get; set; }
        public override Color NameColor => BackgroundColor;
        public override bool HasExtraByte => true;
        public override void SetData()
        {
            ICustomRole customRole = Winner.Data.Role.CustomRole();
            if (customRole != null)
            {
                WinText = customRole.Configuration.NeutralWinText;
                BackgroundColor = customRole.RoleColor;
                return;
            }
            WinText = $"{FungleTranslation.VictoryText.GetString()} " + customRole.RoleName.GetString();
            BackgroundColor = Winner.Data.Role.NameColor;
        }
        public override void ReceiveDataFromRpcEndGame(PlayerControl data)
        {
            Winner = data;
        }
        public override byte GetExtraByte()
        {
            return Winner.PlayerId;
        }
        public override void InterpretExtraByte(byte b)
        {
            Winner = PlayerExtensions.GetPlayerById(b);
        }
    }
}
