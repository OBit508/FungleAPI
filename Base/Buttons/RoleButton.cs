using AmongUs.GameOptions;
using FungleAPI.Attributes;
using FungleAPI.Hud;
using FungleAPI.Role;
using FungleAPI.Utilities;

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
        public T Role => GetRole();
        public override bool Active => Player.Data.RoleType == CustomRoleManager.GetRoleType<T>();
        private T __role;
        private T GetRole()
        {
            if (__role == null)
            {
                RoleBehaviour roleBehaviour = Player != null && Player.Data != null ? Player.Data.Role : null; ;
                if (roleBehaviour.Is(out T result))
                {
                    __role = result;
                }
            }
            return __role;
        }
    }
}
