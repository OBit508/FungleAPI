using AmongUs.GameOptions;
using FungleAPI.Attributes;
using FungleAPI.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FungleAPI.GModes.Logics
{
    [RegisterTypeInIl2Cpp]
    internal class LOptions : LogicOptionsNormal
    {
        public LOptions(GameManager gameManager) : base(gameManager) { }
        public LOptions(IntPtr intPtr) : base(intPtr) { }
    }
}
