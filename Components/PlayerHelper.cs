using FungleAPI.Player;
using FungleAPI.Utilities.Assets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace FungleAPI.Components
{
    public class PlayerHelper : PlayerComponent
    {
        public RoleBehaviour OldRole = RoleManager.Instance.GetRole(AmongUs.GameOptions.RoleTypes.Crewmate);
        internal Vent __CurrentVent;
        public Vent CurrentVent => player.AmOwner ? Vent.currentVent : __CurrentVent;
    }
}
