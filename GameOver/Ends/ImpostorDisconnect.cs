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
    public class ImpostorDisconnect : CustomGameOver
    {
        public override string WinText => StringNames.ImpostorDisconnected.GetString();
        public override Color BackgroundColor { get; } = Color.red;
        public override Color NameColor { get; } = Palette.CrewmateBlue;
        public override GameOverReason Reason => GameOverReason.ImpostorDisconnect;
    }
}
