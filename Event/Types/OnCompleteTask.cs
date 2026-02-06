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
    public class OnCompleteTask : FungleEvent
    {
        /// <summary>
        /// 
        /// </summary>
        public PlayerControl Player { get; internal set; }
        /// <summary>
        /// 
        /// </summary>
        public PlayerTask Task { get; internal set; }
    }
}
