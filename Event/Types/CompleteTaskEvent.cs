using FungleAPI.Base.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FungleAPI.Event.Types
{
    public class CompleteTaskEvent : FungleEvent
    {
        public readonly uint Index;
        public CompleteTaskEvent(uint index)
        {
            Index = index;
        }
    }
}
