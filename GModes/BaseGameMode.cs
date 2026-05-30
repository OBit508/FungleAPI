using AmongUs.GameOptions;
using FungleAPI.Attributes;
using FungleAPI.Extensions;
using FungleAPI.GameOver;
using FungleAPI.GameOver.Ends;
using FungleAPI.Player;
using FungleAPI.PluginLoading;
using FungleAPI.Role;
using FungleAPI.Role.Utilities;
using FungleAPI.Teams;
using FungleAPI.Utilities;
using GameCore;
using InnerNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FungleAPI.GModes
{
    [FungleIgnore]
    public abstract class BaseGameMode
    {
        public abstract StringNames GameModeName { get; }
        public uint GameModeId { get; internal set; }
        public GameManager Manager => GameManager.Instance;
        public IGameOptions GameOptions => Manager.LogicOptions.currentGameOptions;
        public virtual GameModeOptions ModeOptions { get; }
        public virtual bool CanUse(IUsable usable, PlayerControl player) => true;
        public virtual void OnPlayerDeath(PlayerControl player, bool assignGhostRole) { }
        public virtual int GetVotingTime() => 0;
        public virtual int GetDiscussionTime() => 0;
        public virtual bool GetShowCrewmateNames() => true;
        public virtual float GetEngineerInVentTime() => 0;
        public virtual float GetEngineerCooldown() => 0;
        public virtual TaskBarMode GetTaskBarMode() => default;
        public virtual bool GetAnonymousVotes() => false;
        public virtual bool GetVisualTasks() => true;
        public virtual int GetNumEmergencyMeetings() => 1;
        public virtual int GetEmergencyCooldown() => 0;
        public virtual bool GetConfirmImpostor() => true;
        public virtual float GetPlayerSpeedMod(PlayerControl pc) => 0;
        public virtual float GetKillDistance() => 0;
        public virtual float GetKillCooldown() => 0;
        public virtual bool GetGhostsDoTasks() => false;
        public virtual void OnMinigameOpen() { }
        public virtual void OnMinigameClose() { }
        public virtual float CanUseVent(Vent vent, NetworkedPlayerInfo pc, out bool canUse, out bool couldUse) { canUse = false; couldUse = false; return default; }
        public virtual MapOptions GetMapOptions() => null;
        public virtual DeadBody GetDeadBody(GameManager gameManager, RoleBehaviour impostorRole) => null;
        public virtual void SelectRoles(RoleManager roleManager) { }
        public virtual void AssignTasks(ShipStatus shipStatus) { }
        public virtual void CheckEndCriteria() { }
        public virtual void Initialize(ModPlugin modPlugin) { }
    }
}
