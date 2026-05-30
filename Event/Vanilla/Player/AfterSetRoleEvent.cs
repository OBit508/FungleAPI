using AmongUs.GameOptions;
using FungleAPI.Base.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FungleAPI.Event.Vanilla.Player
{
    public class AfterSetRoleEvent : FungleEvent
    {
        public readonly PlayerControl TargetPlayer;
        public readonly RoleTypes RoleType;
        public AfterSetRoleEvent(PlayerControl targetPlayer, RoleTypes roleType)
        {
            TargetPlayer = targetPlayer;
            RoleType = roleType;
        }
    }
}
