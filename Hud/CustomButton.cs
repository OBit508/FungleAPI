using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FungleAPI.Hud
{
    /// <summary>
    /// A class to easily retrieve the instance of a given type
    /// </summary>
    public class CustomButton<TButton> where TButton : CustomAbilityButton
    {
        /// <summary>
        /// The instance
        /// </summary>
        public static TButton Instance => HudHelper.GetButtonInstance<TButton>();
    }
}
