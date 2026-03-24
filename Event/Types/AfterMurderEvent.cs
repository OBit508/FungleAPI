using FungleAPI.Base.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FungleAPI.Event.Types
{
    public class AfterMurderEvent : FungleEvent
    {
        public readonly PlayerControl Source;
        public readonly PlayerControl Target;
        public readonly DeadBody Body;
        public readonly MurderResultFlags Flags;
        public AfterMurderEvent(PlayerControl source, PlayerControl target, DeadBody body, MurderResultFlags flags)
        {
            Source = source;
            Target = target;
            Body = body;
            Flags = flags;
        }
    }
}
