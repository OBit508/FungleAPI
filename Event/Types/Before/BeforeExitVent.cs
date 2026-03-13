using FungleAPI.Base.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FungleAPI.Event.Types.Before
{
    public class BeforeExitVent : CancelableEvent
    {
        public readonly PlayerControl Source;
        public readonly int VentId;
        public BeforeExitVent(PlayerControl source, int ventId)
        {
            Source = source;
            VentId = ventId;
        }
    }
}
