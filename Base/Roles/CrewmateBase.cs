using FungleAPI.Attributes;
using FungleAPI.Player;
using FungleAPI.Role;
using FungleAPI.Utilities;
using UnityEngine;
using System.Collections.Generic;

namespace FungleAPI.Base.Roles
{
    /// <summary>
    /// Base class to create a crewmate role
    /// </summary>
    [FungleIgnore]
    public class CrewmateBase : RoleBaseHelper
    {
        public override bool IsDead => false;
        public override bool CanUse(IUsable usable)
        {
            return usable.SafeCast<ZiplineConsole>() != null || usable.SafeCast<Ladder>() != null || usable.SafeCast<PlatformConsole>() != null || usable.SafeCast<Console>() != null || usable.SafeCast<DoorConsole>() != null;
        }
        public override bool DidWin(GameOverReason gameOverReason)
        {
            return GameManager.Instance.DidHumansWin(gameOverReason);
        }
    }
}
