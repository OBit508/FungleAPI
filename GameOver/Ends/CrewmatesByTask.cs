using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FungleAPI.GameOver.Ends
{
    public class CrewmatesByTask : CrewmatesByVote
    {
        public override GameOverReason Reason => GameOverReason.CrewmatesByTask;
    }
}
