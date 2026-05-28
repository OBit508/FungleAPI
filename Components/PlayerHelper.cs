using AmongUs.GameOptions;
using FungleAPI.Player.Networking;
using FungleAPI.Role;

namespace FungleAPI.Components
{
    /// <summary>
    ///  A component designed to help the API work
    /// </summary>
    public class PlayerHelper : PlayerComponent
    {
        /// <summary>
        /// Returns to the last role the player had when alive
        /// </summary>
        public RoleTypes LastAliveRole = RoleTypes.Crewmate;
        /// <summary>
        /// Returns to the last role the player had when dead
        /// </summary>
        public RoleTypes LastDeadRole = RoleTypes.CrewmateGhost;
        internal Vent __CurrentVent;
        /// <summary>
        /// Returns the player's current vent
        /// </summary>
        public Vent CurrentVent => player.AmOwner ? Vent.currentVent : __CurrentVent;
        public void Update()
        {
            RoleConfigManager.LightConfig?.Update?.Invoke();
        }
    }
}
