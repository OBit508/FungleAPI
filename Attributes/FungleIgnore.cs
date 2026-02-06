using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FungleAPI.Attributes
{
    /// <summary>
    /// This attribute causes the class to which it was assigned to be ignored in auto-registration
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, Inherited = false)]
    public class FungleIgnore : Attribute
    {
    }
}
