using FungleAPI.Base.Roles;
using FungleAPI.Role;
using FungleAPI.Role.Utilities;
using FungleAPI.Teams;
using FungleAPI.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FungleAPI.ModCompatibility.MiraSupport
{
    public class MiraRoleExtensions
    {
        public virtual bool IsMiraRole(RoleBehaviour roleBehaviour) => false;
        public virtual ModdedTeam GetTeam(RoleBehaviour role) => null;
        public virtual RoleHintType GetHint(RoleBehaviour role) => RoleHintType.PlayerTab;
        public virtual bool CanSabotage(RoleBehaviour roleBehaviour) => false;
        public virtual bool UseKillButton(RoleBehaviour roleBehaviour) => false;
        public virtual bool CanUseVent(RoleBehaviour roleBehaviour) => false;
        public virtual bool CanLocalPlayerSeeRole(RoleBehaviour roleBehaviour, PlayerControl player) => false;
        public virtual void AppendHint(RoleBehaviour roleBehaviour, Il2CppSystem.Text.StringBuilder stringBuilder) { }
    }
}
