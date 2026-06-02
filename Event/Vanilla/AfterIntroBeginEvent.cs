using FungleAPI.Base.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FungleAPI.Event.Vanilla
{
    public class AfterIntroBeginEvent : FungleEvent
    {
        public readonly IntroCutscene Intro;
        public AfterIntroBeginEvent(IntroCutscene introCutscene)
        {
            Intro = introCutscene;
        }
    }
}
