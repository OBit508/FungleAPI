using FungleAPI.Translation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FungleAPI.Modifiers
{
    internal class TestModifier : BaseModifier
    {
        public override StringNames ModifierName => TranslationManager.GetStringName("test");
        public override StringNames ModifierBlur => TranslationManager.GetStringName("test");
        public override bool ForceCanKill => true;
        public override bool ForceCanSabotage => true;
        public override bool ForceCanVent => true;
    }
}
