using FungleAPI.GameModes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace FungleAPI.ModCompatibility.MiraSupport
{
    public class MiraModeBridge
    {
        public virtual IEnumerable<BaseGameMode> GetMiraGameModes() => null;
        public virtual bool IsMiraMode(BaseGameMode baseGameMode) => false;
        public virtual List<NetworkedPlayerInfo> GetOverrideWinners() => null;
        public virtual void CanKill(ref bool runOriginal, ref bool result, PlayerControl target) { }
        public virtual bool CanReport(DeadBody body) => false;
        public virtual System.Collections.IEnumerator CoPostHudStart(HudManager hudManager) { yield break; }
        public virtual void HudUpdate(HudManager hudManager) { }
        public virtual void UpdateTaskPanel(TaskPanelBehaviour instance) { }
        public virtual bool BuildViewTab(BaseGameMode baseGameMode, LobbyViewSettingsPane lobbyViewSettingsPane) => false;
        public virtual bool BuildEditTab(BaseGameMode baseGameMode, GameOptionsMenu gameOptionsMenu) => false;
    }
}
