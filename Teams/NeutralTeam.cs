using FungleAPI.GameOver;
using FungleAPI.GameOver.Ends;
using FungleAPI.Translation;
using FungleAPI.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace FungleAPI.Teams
{
    /// <summary>
    /// 
    /// </summary>
    public class NeutralTeam : ModdedTeam
    {
        public override bool FriendlyFire => true;
        public override bool KnowMembers => false;
        public override Color TeamColor => Color.gray;
        public override StringNames TeamName { get; } = FungleTranslation.NeutralText;
        public override StringNames PluralName { get; } = FungleTranslation.NeutralsText;
        public override string VictoryText => FungleTranslation.NeutralGameOver.GetString();
        public override CustomGameOver DefaultGameOver => GameOverManager.GetGameOverInstance<NeutralGameOver>();
        public override uint MaxCount => 50;
    }
}
