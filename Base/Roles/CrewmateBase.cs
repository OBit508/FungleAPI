using FungleAPI.Attributes;
using FungleAPI.Player;
using FungleAPI.Role;
using FungleAPI.Utilities;
using UnityEngine;
using System.Collections.Generic;

namespace FungleAPI.Base.Roles
{
    [FungleIgnore]
    public class CrewmateBase : RoleBaseHelper
    {
        public virtual bool DoTasks => false;
        public override bool IsDead => false;
        public override bool CanUse(IUsable usable)
        {
            if (usable.SafeCast<ZiplineConsole>() != null || usable.SafeCast<Ladder>() != null || usable.SafeCast<PlatformConsole>() != null || usable.SafeCast<Console>() != null || usable.SafeCast<DoorConsole>() != null)
            {
                return DoTasks;
            }
            return true;
        }
        public override bool DidWin(GameOverReason gameOverReason)
        {
            return GameManager.Instance.DidHumansWin(gameOverReason);
        }
    }
}
