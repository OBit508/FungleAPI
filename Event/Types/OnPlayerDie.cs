using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FungleAPI.Event.Types
{
    /// <summary>
    /// 
    /// </summary>
    public class OnPlayerDie : FungleEvent
    {
        /// <summary>
        /// 
        /// </summary>
        public PlayerControl Player { get; internal set; }
        /// <summary>
        /// 
        /// </summary>
        public DeathReason Reason { get; internal set; }
    }
}
