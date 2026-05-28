using FungleAPI.Base.Events;
using FungleAPI.GameOver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FungleAPI.Event.Vanilla
{
    public class AfterGameOver : FungleEvent
    {
        public readonly BaseGameOver GameOver;
        public AfterGameOver(BaseGameOver gameOver)
        {
            GameOver = gameOver;
        }
    }
}
