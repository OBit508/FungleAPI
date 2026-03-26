using FungleAPI.Base.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FungleAPI.Event.Vanilla
{
    public class EndMeetingEvent : FungleEvent
    {
        public readonly MeetingHud Meeting;
        public EndMeetingEvent(MeetingHud meeting)
        {
            Meeting = meeting;
        }
    }
}
