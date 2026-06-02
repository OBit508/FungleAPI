using FungleAPI.Base.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FungleAPI.Event.Vanilla
{
    public class BeforeIntroBeginEvent : CancelableEvent
    {
        public readonly IntroCutscene Intro;
        public BeforeIntroBeginEvent(IntroCutscene introCutscene)
        {
            Intro = introCutscene;
        }
    }
}
