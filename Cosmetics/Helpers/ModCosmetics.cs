using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FungleAPI.Attributes;

namespace FungleAPI.Cosmetics.Helpers
{
    /// <summary>
    /// 
    /// </summary>
    [FungleIgnore]
    public class ModCosmetics
    {
        public virtual List<CustomColor> Colors { get; }
    }
}
