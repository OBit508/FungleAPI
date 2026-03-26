using FungleAPI.Base.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FungleAPI.Event.Vanilla
{
    public class BeforeEjectionEvent : CancelableEvent
    {
        public readonly ExileController ExileController;
        public readonly ExileController.InitProperties InitData;
        public BeforeEjectionEvent(ExileController exileController, ExileController.InitProperties initData)
        {
            ExileController = exileController;
            InitData = initData;
        }
    }
}
