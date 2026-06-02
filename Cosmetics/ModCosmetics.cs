using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FungleAPI.Attributes;
using FungleAPI.Cosmetics.Colors;
using FungleAPI.Cosmetics.Hats;

namespace FungleAPI.Cosmetics
{
    /// <summary>
    /// Class used to add custom cosmetics
    /// </summary>
    [FungleIgnore]
    public class ModCosmetics
    {
        public virtual CustomColor[] Colors { get; protected set; }
        public virtual CustomHat[] Hats { get; protected set; }
        public virtual void Initialize() { }
    }
}
