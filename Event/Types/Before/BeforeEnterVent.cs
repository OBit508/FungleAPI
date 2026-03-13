using FungleAPI.Base.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FungleAPI.Event.Types.Before
{
    public class BeforeEnterVent : CancelableEvent
    {
        public readonly PlayerControl Source;
        public readonly Vent Vent;
        public BeforeEnterVent(PlayerControl source, Vent vent)
        {
            Source = source;
            Vent = vent;
        }
    }
}
