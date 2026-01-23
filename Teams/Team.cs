using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FungleAPI.Teams
{
    public static class Team<TeamT> where TeamT : ModdedTeam
    {
        public static TeamT Instance => ModdedTeam.Instance<TeamT>();
    }
}
