using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FungleAPI.Event.Types
{
    /// <summary>
    /// 
    /// </summary>
    public class OnIntroBegin : FungleEvent
    {
        /// <summary>
        /// 
        /// </summary>
        public IntroCutscene Intro { get; internal set; }
    }
}
