using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace FungleAPI.Role.Teams
{
    public class CrewmateTeam : ModdedTeam
    {
        public override bool FriendlyFire => true;
        public override bool KnowMembers => false;
        public override Color TeamColor => Palette.CrewmateBlue;
        public override Color TeamHeaderColor => Palette.CrewmateRoleHeaderBlue;
        public override StringNames TeamName => StringNames.Crewmate;
        public override StringNames PluralName => StringNames.Crewmates;
        public override List<GameOverReason> WinReason { get; } = new List<GameOverReason>() { GameOverReason.CrewmatesByVote, GameOverReason.CrewmatesByTask, GameOverReason.CrewmateDisconnect, GameOverReason.HideAndSeek_CrewmatesByTimer };
    }
}
