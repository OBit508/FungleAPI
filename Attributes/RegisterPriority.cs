using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FungleAPI.Attributes
{
    /// <summary>
    /// This attribute gives a priority on the auto-registration
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, Inherited = false)]
    public class RegisterPriority : Attribute
    {
        public int Priority { get; }
        public RegisterPriority(int priority)
        {
            Priority = priority;
        }
    }
}
