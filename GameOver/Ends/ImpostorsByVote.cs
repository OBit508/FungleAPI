using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FungleAPI.GameOver.Ends
{
    public class ImpostorsByVote : ImpostorsByKill
    {
        public override GameOverReason Reason => GameOverReason.ImpostorsByVote;
    }
}
