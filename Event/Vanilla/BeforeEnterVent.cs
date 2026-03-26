using FungleAPI.Base.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FungleAPI.Event.Vanilla
{
    public class BeforeEnterVent : CancelableEvent
    {
        public readonly int VentId;
        public BeforeEnterVent(int ventId)
        {
            VentId = ventId;
        }
    }
}
