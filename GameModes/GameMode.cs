using FungleAPI.Hud;
using FungleAPI.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FungleAPI.GameModes
{
    /// <summary>
    /// A class to easily get the instance of a given type
    /// </summary>
    public class GameMode<TGameMode> where TGameMode : BaseGameMode
    {
        private static TGameMode __mode;
        /// <summary>
        /// The instance
        /// </summary>
        public static TGameMode Instance
        {
            get
            {
                if (__mode == null)
                {
                    __mode = GameModeManager.GameModes.Values.FirstOrDefault(g => g.GetType() == typeof(TGameMode)).SimpleCast<TGameMode>();
                }
                return __mode;
            }
        }
    }
}
