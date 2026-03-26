using FungleAPI.Base.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FungleAPI.Event.Vanilla
{
    public class StartMeetingEvent : FungleEvent
    {
        public readonly MeetingHud Meeting;
        public StartMeetingEvent(MeetingHud meeting)
        {
            Meeting = meeting;
        }
    }
}
