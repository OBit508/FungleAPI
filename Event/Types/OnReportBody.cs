using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FungleAPI.Event.Types
{
    public class OnReportBody : FungleEvent
    {
        public PlayerControl Reporter { get; internal set; }
        public NetworkedPlayerInfo Target { get; internal set; }
        public DeadBody Body { get; internal set; }
    }
}
