using FungleAPI.Attributes;
using FungleAPI.Hud;

namespace FungleAPI.Base.Buttons
{
    [FungleIgnore]
    public abstract class RoleButton<T> : CustomAbilityButton where T : RoleBehaviour
    {
        public PlayerControl Player => PlayerControl.LocalPlayer;
        public RoleBehaviour Role => Player != null && Player.Data != null ? Player.Data.Role : null;
        public override bool Active => Role != null && Role.GetType() == typeof(T);
    }
}
