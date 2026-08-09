using FungleAPI.Translation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace FungleAPI.Modifiers
{
    internal class ExampleModifier : BaseModifier
    {
        public override StringNames ModifierName => TranslationManager.GetStringName("Example");
        public override StringNames ModifierBlur => TranslationManager.GetStringName("Uhhhh");
        public override Color ModifierColor => Color.blue;
    }
}
