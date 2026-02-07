using FungleAPI.Attributes;
using FungleAPI.Hud;

namespace FungleAPI.Base.Buttons
{
    /// <summary>
    /// Base class to create a button for a role
    /// </summary>
    /// <typeparam name="T">Role assigned to the button</typeparam>
    [FungleIgnore]
    public abstract class RoleButton<T> : CustomAbilityButton where T : RoleBehaviour
    {
        /// <summary>
        /// Returns the local player
        /// </summary>
        public PlayerControl Player => PlayerControl.LocalPlayer;
        /// <summary>
        /// Returns the local player's role
        /// </summary>
        public RoleBehaviour Role => Player != null && Player.Data != null ? Player.Data.Role : null;
        public override bool Active => Role != null && Role.GetType() == typeof(T);
    }
}
