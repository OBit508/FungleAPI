using FungleAPI.Base.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FungleAPI.Event.Types.Before
{
    public class BeforeMoveVent : CancelableEvent
    {
        public readonly Vent StarterVent;
        public readonly Vent TargetVent;
        public readonly PlayerControl Source;
        public BeforeMoveVent(Vent starterVent, Vent targetVent, PlayerControl source)
        {
            StarterVent = starterVent;
            TargetVent = targetVent;
            Source = source;
        }
    }
}
