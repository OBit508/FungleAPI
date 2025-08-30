using FungleAPI.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace FungleAPI.MonoBehaviours
{
    [RegisterTypeInIl2Cpp]
    public class Updater : MonoBehaviour
    {
        public Action update;
        public Action fixedUpdate;
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
