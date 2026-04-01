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
        public readonly CustomGameOver GameOver;
        public BeforeGameOver(CustomGameOver gameOver)
        {
            GameOver = gameOver;
        }
    }
}
