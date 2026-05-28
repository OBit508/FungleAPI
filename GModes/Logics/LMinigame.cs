using FungleAPI.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FungleAPI.GModes.Logics
{
    [RegisterTypeInIl2Cpp]
    internal class LMinigame : LogicMinigame
    {
        public LMinigame(GameManager gameManager) : base(gameManager) { }
        public LMinigame(IntPtr intPtr) : base(intPtr) { }
        public override void OnMinigameClose()
        {
            GameModeManager.GetCurrentGameMode().OnMinigameClose();
        }
        public override void OnMinigameOpen()
        {
            GameModeManager.GetCurrentGameMode().OnMinigameOpen();
        }
    }
}
