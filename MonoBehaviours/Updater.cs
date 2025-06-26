using FungleAPI.Rpc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace FungleAPI.MonoBehaviours
{
    public class Updater : MonoBehaviour
    {
        public Action onUpdate;
        public void Update()
        {
            if (onUpdate != null)
            {
                onUpdate();
            }
        }
    }
}
