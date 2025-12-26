using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FungleAPI.Event.Types
{
    public class OnStartMeeting : FungleEvent
    {
        public MeetingHud Meeting { get; internal set; }
    }
}
