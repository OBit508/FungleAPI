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
    public class ModdedVent : MonoBehaviour
    {
        public List<Vent> NearbyVents = new List<Vent>();
    }
}
