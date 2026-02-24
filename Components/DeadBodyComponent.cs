using FungleAPI.Attributes;
using FungleAPI.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace FungleAPI.Components
{
    /// <summary>
    /// A base component for creating components that are automatically added to dead bodies
    /// </summary>
    [FungleIgnore]
    public class DeadBodyComponent : MonoBehaviour
    {
        public DeadBody deadBody;
    }
}
