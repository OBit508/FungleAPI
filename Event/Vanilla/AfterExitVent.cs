using FungleAPI.Base.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FungleAPI.Event.Vanilla
{
    public class AfterExitVent : FungleEvent
    {
        public readonly PlayerControl Source;
        public readonly Vent Vent;
        public AfterExitVent(PlayerControl source, Vent vent)
        {
            Source = source;
            Vent = vent;
        }
    }
}
