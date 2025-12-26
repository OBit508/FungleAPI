using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FungleAPI.Event.Types
{
    public class OnUpdateSystem : FungleEvent
    {
        public SystemTypes SystemType { get; internal set; }
        public PlayerControl Player { get; internal set; }
        public byte Amount { get; internal set; }
    }
}
