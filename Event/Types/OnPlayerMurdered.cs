using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FungleAPI.Event.Types
{
    public class OnPlayerMurdered : FungleEvent
    {
        public MurderResultFlags ResultFlags { get; internal set; }
        public PlayerControl Killer { get; internal set; }
        public PlayerControl Target { get; internal set; }
        public DeadBody Body { get; internal set; }
    }
}
