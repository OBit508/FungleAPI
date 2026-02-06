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
    public class OnEndMeeting : FungleEvent
    {
        /// <summary>
        /// 
        /// </summary>
        public MeetingHud Meeting { get; internal set; }
    }
}
