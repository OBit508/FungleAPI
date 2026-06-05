using FungleAPI.Attributes;
using FungleAPI.Player;
using FungleAPI.Utilities;
using UnityEngine;
using System.Collections.Generic;
using FungleAPI.Role.Utilities;

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
            return base.CanUse(usable) || usable.SafeCast<Vent>() != null;
        }
        public override bool DidWin(GameOverReason gameOverReason)
        {
            return GameManager.Instance.DidHumansWin(gameOverReason);
        }
    }
}
