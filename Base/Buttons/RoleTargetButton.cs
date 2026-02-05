using FungleAPI.Attributes;

namespace FungleAPI.Base.Buttons
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TTarget"></typeparam>
    /// <typeparam name="TRole"></typeparam>
    [FungleIgnore]
    public abstract class RoleTargetButton<TTarget, TRole> : TargetButton<TTarget> where TTarget : UnityEngine.Object where TRole : RoleBehaviour
    {
        /// <summary>
        /// 
        /// </summary>
        public PlayerControl Player => PlayerControl.LocalPlayer;
        /// <summary>
        /// 
        /// </summary>
        public RoleBehaviour Role => Player != null && Player.Data != null ? Player.Data.Role : null;
        /// <summary>
        /// 
        /// </summary>
        public override bool Active => Role != null && Role.GetType() == typeof(TRole);
    }
}
