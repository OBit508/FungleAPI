using AmongUs.GameOptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FungleAPI.Player.Networking.Data
{
    internal struct SetRoleData
    {
        public PlayerControl Source;
        public RoleTypes RoleType;
        public bool ShowIntro;
    }
}
