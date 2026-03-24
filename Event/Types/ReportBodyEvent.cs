using FungleAPI.Base.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FungleAPI.Event.Types
{
    public class ReportBodyEvent : FungleEvent
    {
        public readonly PlayerControl Source;
        public readonly NetworkedPlayerInfo Target;
        public ReportBodyEvent(PlayerControl source, NetworkedPlayerInfo target)
        {
            Source = source;
            Target = target;
        }
    }
}
