using FungleAPI.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FungleAPI.Teams
{
    public static class ModdedTeamManager
    {
        public static Dictionary<Type, ModdedTeam> Teams = new Dictionary<Type, ModdedTeam>();
        /// <summary>
        /// Gets the Crewmates team instance
        /// </summary>
        public static ModdedTeam Crewmates => GetTeamInstance<CrewmateTeam>();
        /// <summary>
        /// Gets the Impostors team instance
        /// </summary>
        public static ModdedTeam Impostors => GetTeamInstance<ImpostorTeam>();
        /// <summary>
        /// Gets the Neutrals team instance
        /// </summary>
        public static ModdedTeam Neutrals => GetTeamInstance<NeutralTeam>();
        /// <summary>
        /// Returns the instance of the given type
        /// </summary>
        public static T GetTeamInstance<T>() where T : ModdedTeam
        {
            return GetTeamInstance(typeof(T)).SimpleCast<T>() ?? null;
        }
        /// <summary>
        /// Returns the instance of the given type
        /// </summary>
        public static ModdedTeam GetTeamInstance(Type type)
        {
            if (Teams.TryGetValue(type, out ModdedTeam moddedTeam))
            {
                return moddedTeam;
            }
            return null;
        }
        /// <summary>
        /// Returns the instance of the given id
        /// </summary>
        public static ModdedTeam GetTeamInstance(int id)
        {
            return Teams.Values.FirstOrDefault(t => t.TeamId == id);
        }
    }
}
