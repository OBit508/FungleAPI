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
    public class OnCloseDoor : FungleEvent
    {
        /// <summary>
        /// 
        /// </summary>
        public SystemTypes Room { get; internal set; }
    }
}
