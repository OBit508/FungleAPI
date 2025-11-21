using FungleAPI.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FungleAPI.Base.Buttons
{
    [FungleIgnore]
    public class RoleTargetButton<TTarget, TRole> : TargetButton<TTarget> where TTarget : UnityEngine.Object where TRole : RoleBehaviour
    {
        public PlayerControl Player => PlayerControl.LocalPlayer;
        public RoleBehaviour Role => Player != null && Player.Data != null ? Player.Data.Role : null;
        public override bool Active => Role != null && Role.GetType() == typeof(TRole);
        public override bool CanUse => Target != null;
        public override bool CanClick => CanUse;
    }
}
