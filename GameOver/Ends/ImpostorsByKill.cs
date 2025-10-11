using FungleAPI.Translation;
using FungleAPI.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace FungleAPI.GameOver.Ends
{
    public class ImpostorsByKill : CustomGameOver
    {
        public override string WinText => FungleTranslation.ImpostorGameOver.GetString();
        public override Color BackgroundColor { get; } = Color.red;
        public override Color NameColor { get; } = Color.red;
        public override GameOverReason Reason => GameOverReason.ImpostorsByKill;
    }
}
