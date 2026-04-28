using AmongUs.GameOptions;
using FungleAPI.Attributes;
using FungleAPI.Player;
using FungleAPI.Role;
using FungleAPI.Role.Utilities;
using FungleAPI.Utilities;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

namespace FungleAPI.Base.Roles
{
    /// <summary>
    /// Base class to create o impostor role
    /// </summary>
    [FungleIgnore]
    public class ImpostorBase : RoleBaseHelper
    {
        /// <summary>
        /// Indicates whether the player with this role can open and use task consoles
        /// </summary>
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
                StringNames.ImpostorTask.GetString(),
                "\r\n",
                Palette.ImpostorRed.ToTextColor(),
                StringNames.FakeTasks.GetString(),
                "</color>"
                    });
                    return;
                case GameModes.HideNSeek:
                case GameModes.SeekFools:
                    StringNames.RuleOneImpostor.GetString();
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
            if (!this.CanUseVent() && usable.SafeCast<Vent>() != null)
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
