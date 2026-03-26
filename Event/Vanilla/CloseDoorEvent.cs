using FungleAPI.Base.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FungleAPI.Event.Vanilla
{
    public class CloseDoorEvent : FungleEvent
    {
        public readonly SystemTypes Room;
        public CloseDoorEvent(SystemTypes room)
        {
            Room = room;
        }
    }
}
