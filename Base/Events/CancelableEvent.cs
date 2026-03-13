using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FungleAPI.Base.Events
{
    public class CancelableEvent : FungleEvent
    {
        public bool Cancelled { get; private set; }
        public virtual void Cancel()
        {
            Cancelled = true;
        }
    }
}
