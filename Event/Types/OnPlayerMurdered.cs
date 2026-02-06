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
    public class OnPlayerMurdered : FungleEvent
    {
        /// <summary>
        /// 
        /// </summary>
        public MurderResultFlags ResultFlags { get; internal set; }
        /// <summary>
        /// 
        /// </summary>
        public PlayerControl Killer { get; internal set; }
        /// <summary>
        /// 
        /// </summary>
        public PlayerControl Target { get; internal set; }
        /// <summary>
        /// 
        /// </summary>
        public DeadBody Body { get; internal set; }
    }
}
