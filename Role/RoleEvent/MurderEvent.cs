using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FungleAPI.Role.RoleEvent
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class MurderEvent : Attribute
    {
        public EventTime Time;
        public MurderEvent(EventTime time)
        {
            Time = time;
        }
    }
}
