using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FungleAPI.Attributes
{
    /// <summary>
    /// This attribute automatically registers the class to which it was assigned in Il2cpp
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, Inherited = false)]
    public class RegisterTypeInIl2Cpp : Attribute
    {
    }
}
