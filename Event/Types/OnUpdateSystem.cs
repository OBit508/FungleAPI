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
    public class OnUpdateSystem : FungleEvent
    {
        /// <summary>
        /// 
        /// </summary>
        public SystemTypes SystemType { get; internal set; }
        /// <summary>
        /// 
        /// </summary>
        public PlayerControl Player { get; internal set; }
        /// <summary>
        /// 
        /// </summary>
        public byte Amount { get; internal set; }
    }
}
