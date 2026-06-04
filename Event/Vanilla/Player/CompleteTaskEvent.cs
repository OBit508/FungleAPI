using FungleAPI.Base.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FungleAPI.Event.Vanilla.Player
{
    public class CompleteTaskEvent : FungleEvent
    {
        public readonly PlayerControl Source;
        public readonly uint Index;
        public CompleteTaskEvent(PlayerControl source, uint index)
        {
            Source = source;
            Index = index;
        }
    }
}
