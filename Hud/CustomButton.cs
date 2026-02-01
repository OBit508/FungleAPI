using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FungleAPI.Hud
{
    public class CustomButton<TButton> where TButton : CustomAbilityButton
    {
        public static TButton Instance => HudHelper.GetButtonInstance<TButton>();
    }
}
