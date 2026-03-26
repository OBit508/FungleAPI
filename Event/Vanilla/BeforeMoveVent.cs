using FungleAPI.Base.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FungleAPI.Event.Vanilla
{
    public class BeforeMoveVent : CancelableEvent
    {
        public readonly Vent TargetVent;
        public BeforeMoveVent(Vent targetVent)
        {
            TargetVent = targetVent;
        }
    }
}
