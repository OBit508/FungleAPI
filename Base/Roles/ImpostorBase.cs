using AmongUs.GameOptions;
using FungleAPI.Attributes;
using FungleAPI.Player;
using FungleAPI.Role;
using FungleAPI.Utilities;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

namespace FungleAPI.Base.Roles
{
    [FungleIgnore]
    public class ImpostorBase : RoleBaseHelper
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
        public override bool DidWin(GameOverReason gameOverReason)
        {
            return GameManager.Instance.DidImpostorsWin(gameOverReason);
        }
    }
}
