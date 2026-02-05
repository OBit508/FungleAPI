using FungleAPI.Attributes;
using FungleAPI.Hud;

namespace FungleAPI.Base.Buttons
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [FungleIgnore]
    public abstract class RoleButton<T> : CustomAbilityButton where T : RoleBehaviour
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
        public override bool Active => Role != null && Role.GetType() == typeof(T);
    }
}
