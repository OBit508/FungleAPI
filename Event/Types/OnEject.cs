using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FungleAPI.Event.Types
{
    public class OnEject : FungleEvent
    {
        public ExileController Controller { get; internal set; }
        public NetworkedPlayerInfo Target { get; internal set; }
    }
}
