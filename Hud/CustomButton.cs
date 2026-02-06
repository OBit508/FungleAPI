using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FungleAPI.Hud
{
    /// <summary>
    /// 
    /// </summary>
    public class CustomButton<TButton> where TButton : CustomAbilityButton
    {
        /// <summary>
        /// 
        /// </summary>
        public static TButton Instance => HudHelper.GetButtonInstance<TButton>();
    }
}
