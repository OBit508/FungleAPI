using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FungleAPI.Teams
{
    /// <summary>
    /// A class to easily get the instance of a given type
    /// </summary>
    public static class Team<TeamT> where TeamT : ModdedTeam
    {
        /// <summary>
        /// The instance
        /// </summary>
        public static TeamT Instance => ModdedTeamManager.GetTeamInstance<TeamT>();
    }
}
