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
    /// <summary>
    /// Class equivalent to the value of the GameOverReason enum
    /// </summary>
    public class CrewmatesByVote : BaseGameOver
    {
        public override string WinText => FungleTranslation.CrewmateGameOver.GetString();
        public override Color BackgroundColor { get; set; } = Palette.CrewmateBlue;
        public override Color NameColor { get; set; } = Palette.CrewmateBlue;
    }
}
