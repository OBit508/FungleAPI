using FungleAPI.Base.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FungleAPI.Event.Vanilla
{
    public class UpdateSystemEvent : FungleEvent
    {
        public readonly SystemTypes SystemType;
        public readonly PlayerControl Source;
        public readonly byte Amount;
        public UpdateSystemEvent(SystemTypes systemTypes, PlayerControl source, byte amount)
        {
            SystemType = systemTypes;
            Source = source;
            Amount = amount;
        }
    }
}
