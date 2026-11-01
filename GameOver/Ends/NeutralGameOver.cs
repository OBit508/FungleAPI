using FungleAPI.Api;
using FungleAPI.Networking;
using FungleAPI.Player;
using FungleAPI.Role;
using FungleAPI.Role.Utilities;
using FungleAPI.Teams;
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
        public override bool DidWin(NetworkedPlayerInfo networkedPlayerInfo) => networkedPlayerInfo.Role.GetTeam() == ModdedTeamManager.Neutrals && networkedPlayerInfo.Role.KeepGameRunning() && !networkedPlayerInfo.IsDead;
        public override void SetWinners()
        {
            ICustomRole customRole = Winner.Data.Role.CustomRole();
            if (customRole != null)
            {
                WinText = customRole.NeutralWinText();
                BackgroundColor = customRole.RoleColor;
                return;
            }
            WinText = string.Format(FungleTranslation.VictoryText.GetString(), customRole.RoleName.GetString());
            BackgroundColor = Winner.Data.Role.NameColor;
        }
        public override void Serialize(MessageWriter messageWriter, PlayerControl dataT)
        {
            Winner = dataT;
            messageWriter.WritePlayer(dataT);
        }
        public override void Deserialize(MessageReader messageReader)
        {
            Winner = messageReader.ReadPlayer();
        }
    }
}
