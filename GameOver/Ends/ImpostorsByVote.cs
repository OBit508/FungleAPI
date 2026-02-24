using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FungleAPI.GameOver.Ends
{
    /// <summary>
    /// Class equivalent to the value of the GameOverReason enum
    /// </summary>
    public class ImpostorsByVote : ImpostorsByKill
    {
        public override GameOverReason Reason => GameOverReason.ImpostorsByVote;
    }
}
