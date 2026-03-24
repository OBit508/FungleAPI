using FungleAPI.Base.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FungleAPI.Event.Types
{
    public class BeforeExitVent : CancelableEvent
    {
        public readonly int VentId;
        public BeforeExitVent(int ventId)
        {
            VentId = ventId;
        }
    }
}
