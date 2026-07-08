using FungleAPI.Attributes;
using FungleAPI.GameModes;
using FungleAPI.Player;
using FungleAPI.Role.Utilities;
using FungleAPI.Utilities;
using System.Collections.Generic;
using UnityEngine;

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
            if (!GameModeManager.GetCurrentGameMode().CanUse(usable, Player) && !GameManager.Instance.IsHideAndSeek())
            {
                return false;
            }
            return usable.SafeCast<ZiplineConsole>() != null || usable.SafeCast<Ladder>() != null || usable.SafeCast<PlatformConsole>() != null || usable.SafeCast<Console>() != null || usable.SafeCast<DoorConsole>() != null || usable.SafeCast<Vent>() != null;
        }
        public override bool DidWin(GameOverReason gameOverReason)
        {
            return GameManager.Instance.DidHumansWin(gameOverReason);
        }
    }
}
