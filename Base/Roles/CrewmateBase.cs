using FungleAPI.Attributes;
using FungleAPI.Player;
using FungleAPI.Role;
using FungleAPI.Utilities;

namespace FungleAPI.Base.Roles
{
    [FungleIgnore]
    public class CrewmateBase : RoleBehaviour
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
        public override bool IsValidTarget(NetworkedPlayerInfo target)
        {
            return !(target == null) && !target.Disconnected && !target.IsDead && target.PlayerId != this.Player.PlayerId && !(target.Role == null) && !(target.Object == null) && !target.Object.inVent && !target.Object.inMovingPlat && target.Object.Visible;
        }
        public override PlayerControl FindClosestTarget()
        {
            Il2CppSystem.Collections.Generic.List<PlayerControl> playersInAbilityRangeSorted = GetPlayersInAbilityRangeSorted(GetTempPlayerList());
            if (playersInAbilityRangeSorted.Count <= 0)
            {
                return null;
            }
            return playersInAbilityRangeSorted[0];
        }
        public override bool DidWin(GameOverReason gameOverReason)
        {
            return GameManager.Instance.DidHumansWin(gameOverReason);
        }
        public override DeadBody FindClosestBody()
        {
            return Player.GetClosestDeadBody(GetAbilityDistance());
        }
        public override void AppendTaskHint(Il2CppSystem.Text.StringBuilder taskStringBuilder)
        {
            if (this.GetHintType() == RoleHintType.MiraAPI_RoleTab)
            {
                CustomRoleManager.CreateForRole(taskStringBuilder, this);
                return;
            }
            base.AppendTaskHint(taskStringBuilder);
        }
    }
}
