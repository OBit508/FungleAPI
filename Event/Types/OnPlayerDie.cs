using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FungleAPI.Event.Types
{
    public class OnPlayerDie : FungleEvent
    {
        public PlayerControl Player { get; internal set; }
        public DeathReason Reason { get; internal set; }
    }
}
