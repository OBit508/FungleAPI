using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FungleAPI.Attributes;
using FungleAPI.Cosmetics.Helpers;

namespace FungleAPI.Cosmetics
{
    /// <summary>
    /// Class used to add custom cosmetics
    /// </summary>
    [FungleIgnore]
    public class ModCosmetics
    {
        public virtual List<CustomColor> Colors { get; }
    }
}
