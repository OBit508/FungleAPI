using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FungleAPI.Role.RoleEvent
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class MurderPlayer : Attribute
    {
        public EventTime Time;
        public MurderPlayer(EventTime time)
        {
            Time = time;
        }
    }
}
