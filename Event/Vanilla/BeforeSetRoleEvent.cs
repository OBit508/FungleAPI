using AmongUs.GameOptions;
using FungleAPI.Base.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FungleAPI.Event.Vanilla
{
    public class BeforeSetRoleEvent : CancelableEvent
    {
        public readonly PlayerControl TargetPlayer;
        public readonly RoleTypes RoleType;
        public BeforeSetRoleEvent(PlayerControl targetPlayer, RoleTypes roleType)
        {
            TargetPlayer = targetPlayer;
            RoleType = roleType;
        }
    }
}
