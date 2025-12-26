using AmongUs.GameOptions;
using FungleAPI.Attributes;
using FungleAPI.GameOver;
using FungleAPI.GameOver.Ends;
using FungleAPI.Player;
using FungleAPI.Role;
using FungleAPI.Utilities;
using System.Linq;
using UnityEngine;

namespace FungleAPI.Base.Roles
{
    [FungleIgnore]
    public class NeutralBase : RoleBehaviour
    {
        public virtual bool DoTasks => false;
        public override bool IsDead => false;
        public override void Deinitialize(PlayerControl targetPlayer)
        {
            PlayerTask playerTask = targetPlayer.myTasks.ToSystemList().FirstOrDefault((PlayerTask t) => t.name == "ImpostorRole");
            if (playerTask)
            {
                targetPlayer.myTasks.Remove(playerTask);
                GameObject.Destroy(playerTask.gameObject);
            }
        }
        public override void SpawnTaskHeader(PlayerControl playerControl)
        {
            if (playerControl != PlayerControl.LocalPlayer || !DoTasks)
            {
                return;
            }
            ImportantTextTask orCreateTask = PlayerTask.GetOrCreateTask<ImportantTextTask>(playerControl, 0);
            switch (GameOptionsManager.Instance.CurrentGameOptions.GameMode)
            {
                case GameModes.Normal:
                case GameModes.NormalFools:
                    orCreateTask.Text = string.Concat(new string[]
                    {
                DestroyableSingleton<TranslationController>.Instance.GetString(StringNames.ImpostorTask),
                "\r\n",
                Palette.ImpostorRed.ToTextColor(),
                DestroyableSingleton<TranslationController>.Instance.GetString(StringNames.FakeTasks),
                "</color>"
                    });
                    return;
                case GameModes.HideNSeek:
                case GameModes.SeekFools:
                    orCreateTask.Text = DestroyableSingleton<TranslationController>.Instance.GetString(StringNames.RuleOneImpostor);
                    return;
                default:
                    return;
            }
        }
        public override bool CanUse(IUsable usable)
        {
            if (!GameManager.Instance.LogicUsables.CanUse(usable, this.Player))
            {
                return false;
            }
            Console console = usable.SafeCast<Console>();
            return console == null || DoTasks;
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
            return GameOverManager.Instance<NeutralGameOver>().Reason == gameOverReason;
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
