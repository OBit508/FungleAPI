using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FungleAPI.Event.Types
{
    public class OnCompleteTask : FungleEvent
    {
        public PlayerControl Player { get; internal set; }
        public PlayerTask Task { get; internal set; }
    }
}
