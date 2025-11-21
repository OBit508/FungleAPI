using FungleAPI.Attributes;
using FungleAPI.Hud;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FungleAPI.Base.Buttons
{
    [FungleIgnore]
    public class RoleButton<T> : CustomAbilityButton where T : RoleBehaviour
    {
        public PlayerControl Player => PlayerControl.LocalPlayer;
        public RoleBehaviour Role => Player != null && Player.Data != null ? Player.Data.Role : null;
        public override bool Active => Role != null && Role.GetType() == typeof(T);
    }
}
