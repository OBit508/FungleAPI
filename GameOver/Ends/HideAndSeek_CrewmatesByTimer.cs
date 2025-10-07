using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FungleAPI.GameOver.Ends
{
    public class HideAndSeek_CrewmatesByTimer : CrewmatesByVote
    {
        public override GameOverReason Reason => GameOverReason.HideAndSeek_CrewmatesByTimer;
    }
}
