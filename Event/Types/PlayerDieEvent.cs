using FungleAPI.Base.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FungleAPI.Event.Types
{
    public class PlayerDieEvent : FungleEvent
    {
        public readonly PlayerControl Source;
        public readonly DeathReason Reason;
        public PlayerDieEvent(PlayerControl source, DeathReason reason)
        {
            Source = source;
            Reason = reason;
        }
    }
}
