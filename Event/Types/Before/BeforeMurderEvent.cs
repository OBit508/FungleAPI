using FungleAPI.Base.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FungleAPI.Event.Types.Before
{
    public class BeforeMurderEvent : CancelableEvent
    {
        public readonly PlayerControl Source;
        public readonly PlayerControl Target;
        public readonly MurderResultFlags Flags;
        public BeforeMurderEvent(PlayerControl source, PlayerControl target, MurderResultFlags flags)
        {
            Source = source;
            Target = target;
            Flags = flags;
        }
    }
}
