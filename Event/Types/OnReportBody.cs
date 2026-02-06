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
    public class OnReportBody : FungleEvent
    {
        /// <summary>
        /// 
        /// </summary>
        public PlayerControl Reporter { get; internal set; }
        /// <summary>
        /// 
        /// </summary>
        public NetworkedPlayerInfo Target { get; internal set; }
        /// <summary>
        /// 
        /// </summary>
        public DeadBody Body { get; internal set; }
    }
}
