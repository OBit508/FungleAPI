using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FungleAPI.Event.Types
{
    public class OnEnterVent : FungleEvent
    {
        public PlayerControl Player { get; internal set; }
        public Vent Vent { get; internal set; }
    }
}
