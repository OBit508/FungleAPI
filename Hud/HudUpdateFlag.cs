using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FungleAPI.Hud
{
    public enum HudUpdateFlag
    {
        Always,
        Delay,
        OnSetHudActive,
        DelayAndOnSetHudActive,
        Never
    }
}
