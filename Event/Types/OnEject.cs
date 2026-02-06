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
    public class OnEject : FungleEvent
    {
        /// <summary>
        /// 
        /// </summary>
        public ExileController Controller { get; internal set; }
        /// <summary>
        /// 
        /// </summary>
        public NetworkedPlayerInfo Target { get; internal set; }
    }
}
