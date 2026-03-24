using FungleAPI.Base.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FungleAPI.Event.Types
{
    public class AfterEnterVent : FungleEvent
    {
        public readonly PlayerControl Source;
        public readonly Vent Vent;
        public AfterEnterVent(PlayerControl source, Vent vent)
        {
            Source = source;
            Vent = vent;
        }
    }
}
