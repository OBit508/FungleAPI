using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FungleAPI.Hud
{
    public static class HudHelper
    {
        internal static HudUpdateFlag UpdateFlag = HudUpdateFlag.Always;
        internal static float UpdateDelay;
        internal static bool Active;
        public static void SetUpdateFlag(HudUpdateFlag flag, float delay = 0)
        {
            UpdateFlag = flag;
            UpdateDelay = delay;
        }
    }
}
