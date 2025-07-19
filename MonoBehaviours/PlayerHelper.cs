using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using FungleAPI.Patches;

namespace FungleAPI.MonoBehaviours
{
    public class PlayerHelper : MonoBehaviour
    {
        public RoleBehaviour OldRole = RoleManager.Instance.GetRole(AmongUs.GameOptions.RoleTypes.Crewmate);
    }
}
