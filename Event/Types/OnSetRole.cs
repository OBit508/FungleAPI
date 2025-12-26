using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FungleAPI.Event.Types
{
    public class OnSetRole : FungleEvent
    {
        public RoleBehaviour Role { get; internal set; }
        public PlayerControl Player { get; internal set; }
    }
}
