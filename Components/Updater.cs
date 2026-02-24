using FungleAPI.Attributes;
using Il2CppMono;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.PlayerLoop;

namespace FungleAPI.Components
{
    /// <summary>
    /// A component for applying an Update to an object without needing to create a new component (not recommended)
    /// </summary>
    [RegisterTypeInIl2Cpp]
    public class Updater : MonoBehaviour
    {
        /// <summary>
        /// Called on Update
        /// </summary>
        public Action update;
        /// <summary>
        /// Called on FixedUpdate
        /// </summary>
        public Action fixedUpdate;
        /// <summary>
        /// Called on LateUpdate
        /// </summary>
        public Action lateUpdate;
        public void Update()
        {
            if (update != null)
            {
                update();
            }
        }
        public void FixedUpdate()
        {
            if (fixedUpdate != null)
            {
                fixedUpdate();
            }
        }
        public void LateUpdate()
        {
            if (lateUpdate != null)
            {
                lateUpdate();
            }
        }
    }
}
