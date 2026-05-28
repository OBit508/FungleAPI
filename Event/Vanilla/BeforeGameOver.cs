using FungleAPI.Base.Events;
using FungleAPI.GameOver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FungleAPI.Event.Vanilla
{
    public class BeforeGameOver : CancelableEvent
    {
        public readonly BaseGameOver GameOver;
        public BeforeGameOver(BaseGameOver gameOver)
        {
            GameOver = gameOver;
        }
    }
}
