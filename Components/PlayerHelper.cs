using FungleAPI.Role;

namespace FungleAPI.Components
{
    /// <summary>
    ///  A component designed to help the API work
    /// </summary>
    public class PlayerHelper : PlayerComponent
    {
        /// <summary>
        /// Returns to the last role the player had
        /// </summary>
        public RoleBehaviour OldRole = RoleManager.Instance.GetRole(AmongUs.GameOptions.RoleTypes.Crewmate);
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
