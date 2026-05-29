using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace FungleAPI.Hud
{
    /// <summary>
    /// A class to easily get the instance of a given type
    /// </summary>
    public class CustomButton<TButton> where TButton : CustomAbilityButton
    {
        private static TButton __button;
        /// <summary>
        /// The instance
        /// </summary>
        public static TButton Instance
        {
            get
            {
                if (__button == null)
                {
                    __button = HudHelper.GetButtonInstance<TButton>();
                }
                return __button;
            }
        }
    }
}
