using AmongUs.GameOptions;
using FungleAPI.Attributes;
using FungleAPI.Player;
using FungleAPI.Role;
using FungleAPI.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace FungleAPI.Base.Roles
{
    [FungleIgnore]
    public class ImpostorBase : RoleBehaviour
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
            if (playerControl != PlayerControl.LocalPlayer)
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
            if (!GameManager.Instance.LogicUsables.CanUse(usable, Player))
            {
                return false;
            }
            Console console = usable.SafeCast<Console>();
            return console == null || console.AllowImpostor || DoTasks;
        }
        public virtual Il2CppSystem.Collections.Generic.List<PlayerControl> GetValidTargets()
        {
            List<PlayerControl> targets = GetTempPlayerList().ToSystemList();
            targets.RemoveAll(t => t.Data.Role.GetTeam() == this.GetTeam() && !this.GetTeam().FriendlyFire);
            return targets.ToIl2CppList();
        }
        public override PlayerControl FindClosestTarget()
        {
            Il2CppSystem.Collections.Generic.List<PlayerControl> playersInAbilityRangeSorted = GetPlayersInAbilityRangeSorted(GetValidTargets());
            if (playersInAbilityRangeSorted.Count <= 0)
            {
                return null;
            }
            return playersInAbilityRangeSorted[0];
        }
        public override bool DidWin(GameOverReason gameOverReason)
        {
            return GameManager.Instance.DidImpostorsWin(gameOverReason);
        }
        public override DeadBody FindClosestBody()
        {
            return Player.GetClosestDeadBody(GetAbilityDistance());
        }
    }
}
