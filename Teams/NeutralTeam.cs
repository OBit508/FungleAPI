using FungleAPI.Api;
using FungleAPI.Extensions;
using FungleAPI.GameOver;
using FungleAPI.GameOver.Ends;
using FungleAPI.Role.Utilities;
using FungleAPI.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace FungleAPI.Teams
{
    public class NeutralTeam : ModdedTeam
    {
        public override bool FriendlyFire => true;
        public override bool KnowMembers => false;
        public override Color TeamColor => Color.gray;
        public override StringNames TeamName { get; } = FungleTranslation.NeutralText;
        public override StringNames PluralName { get; } = FungleTranslation.NeutralsText;
        public override string VictoryText => FungleTranslation.NeutralGameOver.GetString();
        public override void CallGameOver(GameManager gameManager) => gameManager.RpcEndGame<NeutralGameOver, PlayerControl>(PlayerControl.AllPlayerControls.FirstOrDefault(p => !p.Data.IsDead && p.Data.Role.KeepGameRunning() && p.Data.Role.GetTeam() == this));
        public override byte MaxCount => 50;
    }
}
