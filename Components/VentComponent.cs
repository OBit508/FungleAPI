using FungleAPI.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace FungleAPI.Components
{
    /// <summary>
    /// A base component for creating components that are automatically added to vents
    /// </summary>
    [FungleIgnore]
    public class VentComponent : MonoBehaviour
    {
        public Vent vent;
    }
}
