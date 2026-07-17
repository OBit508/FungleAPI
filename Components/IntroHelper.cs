using FungleAPI.Attributes;
using FungleAPI.Event;
using FungleAPI.Event.Vanilla;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace FungleAPI.Components
{
    [RegisterTypeInIl2Cpp]
    internal class IntroHelper : MonoBehaviour
    {
        public void Start()
        {
            EventManager.CallEvent(new IntroStartEvent());
        }
        public void OnDestroy()
        {
            EventManager.CallEvent(new IntroEndEvent());
        }
    }
}
