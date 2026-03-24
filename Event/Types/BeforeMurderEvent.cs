using FungleAPI.Base.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FungleAPI.Event.Types
{
    public class BeforeMurderEvent : CancelableEvent
    {
        public readonly PlayerControl Target;
        public readonly MurderResultFlags Flags;
        public BeforeMurderEvent(PlayerControl target, MurderResultFlags flags)
        {
            Target = target;
            Flags = flags;
        }
    }
}
