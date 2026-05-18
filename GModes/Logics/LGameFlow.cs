using FungleAPI.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FungleAPI.GModes.Logics
{
    [RegisterTypeInIl2Cpp]
    internal class LGameFlow : LogicGameFlowNormal
    {
        public LGameFlow(GameManager gameManager) : base(gameManager) { }
        public LGameFlow(IntPtr intPtr) :base(intPtr) {}
    }
}
