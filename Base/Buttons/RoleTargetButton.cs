using FungleAPI.Attributes;

namespace FungleAPI.Base.Buttons
{
    /// <summary>
    /// Base class to create a target button for a role
    /// </summary>
    /// <typeparam name="TTarget">Target type assigned to the button</typeparam>
    /// <typeparam name="TRole">Role assigned to the button</typeparam>
    [FungleIgnore]
    public abstract class RoleTargetButton<TTarget, TRole> : TargetButton<TTarget> where TTarget : UnityEngine.Object where TRole : RoleBehaviour
    {
        /// <summary>
        /// Returns the local player
        /// </summary>
        public PlayerControl Player => PlayerControl.LocalPlayer;
        /// <summary>
        /// Returns the local player's role
        /// </summary>
        public RoleBehaviour Role => Player != null && Player.Data != null ? Player.Data.Role : null;
        public override bool Active => Role != null && Role.GetType() == typeof(TRole);
    }
}
