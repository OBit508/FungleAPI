using AmongUs.GameOptions;
using FungleAPI.Attributes;
using FungleAPI.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace FungleAPI.GModes.Logics
{
    [RegisterTypeInIl2Cpp]
    internal class LOptions : LogicOptionsNormal
    {
        public LOptions(GameManager gameManager) : base(gameManager) { }
        public LOptions(IntPtr intPtr) : base(intPtr) { }
        public override bool GetGhostsDoTasks()
        {
            return GameModeManager.GetCurrentGameMode().GetGhostsDoTasks();
        }
        public override float GetKillCooldown()
        {
            return GameModeManager.GetCurrentGameMode().GetKillCooldown();
        }
        public override float GetKillDistance()
        {
            return GameModeManager.GetCurrentGameMode().GetKillDistance();
        }
        public override float GetPlayerSpeedMod(PlayerControl pc)
        {
            return GameModeManager.GetCurrentGameMode().GetPlayerSpeedMod(pc);
        }
        public override bool GetConfirmImpostor()
        {
            return GameModeManager.GetCurrentGameMode().GetConfirmImpostor();
        }
        public override int GetEmergencyCooldown()
        {
            return GameModeManager.GetCurrentGameMode().GetEmergencyCooldown();
        }
        public override int GetNumEmergencyMeetings()
        {
            return GameModeManager.GetCurrentGameMode().GetNumEmergencyMeetings();
        }
        public override bool GetVisualTasks()
        {
            return GameModeManager.GetCurrentGameMode().GetVisualTasks();
        }
        public override bool GetAnonymousVotes()
        {
            return GameModeManager.GetCurrentGameMode().GetAnonymousVotes();
        }
        public override TaskBarMode GetTaskBarMode()
        {
            return GameModeManager.GetCurrentGameMode().GetTaskBarMode();
        }
        public override float GetEngineerCooldown()
        {
            return GameModeManager.GetCurrentGameMode().GetEngineerCooldown();
        }
        public override float GetEngineerInVentTime()
        {
            return GameModeManager.GetCurrentGameMode().GetEngineerInVentTime();
        }
    }
}
