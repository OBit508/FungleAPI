using FungleAPI.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FungleAPI.GModes.Logics
{
    [RegisterTypeInIl2Cpp]
    internal class LUsasbles : LogicUsablesBasic
    {
        public LUsasbles(GameManager gameManager) : base(gameManager) { }
        public LUsasbles(IntPtr intPtr) : base(intPtr) { }
    }
}
