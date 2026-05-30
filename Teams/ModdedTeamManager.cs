using AmongUs.GameOptions;
using FungleAPI.Extensions;
using FungleAPI.PluginLoading;
using FungleAPI.Translation;
using FungleAPI.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace FungleAPI.Teams
{
    public static class ModdedTeamManager
    {
        internal static uint LastTeamId = uint.MinValue;
        public static Dictionary<Type, ModdedTeam> Teams = new Dictionary<Type, ModdedTeam>();
        /// <summary>
        /// Gets the Crewmates team instance
        /// </summary>
        public static CrewmateTeam Crewmates => Team<CrewmateTeam>.Instance;
        /// <summary>
        /// Gets the Impostors team instance
        /// </summary>
        public static ImpostorTeam Impostors => Team<ImpostorTeam>.Instance;
        /// <summary>
        /// Gets the Neutrals team instance
        /// </summary>
        public static NeutralTeam Neutrals => Team<NeutralTeam>.Instance;
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
        internal static void SetID(ModdedTeam moddedTeam)
        {
            
        }
        public static void RegisterTeam(Type type, ModPlugin plugin)
        {
            ModdedTeam team = (ModdedTeam)Activator.CreateInstance(type);
            plugin.Teams.Add(team);
            Teams.Add(type, team);
            team.TeamId = LastTeamId;
            LastTeamId++;
            team.CountData = ScriptableObject.CreateInstance<FloatGameSetting>().DontUnload();
            team.CountData.Type = OptionTypes.Float;
            team.CountData.Title = FungleTranslation.CountText;
            team.CountData.Increment = 1;
            team.CountData.ValidRange = new FloatRange(0, team.MaxCount);
            team.CountData.FormatString = null;
            team.CountData.ZeroIsInfinity = false;
            team.CountData.SuffixType = NumberSuffixes.None;
            team.CountData.OptionName = FloatOptionNames.Invalid;
            team.PriorityData = ScriptableObject.CreateInstance<FloatGameSetting>().DontUnload();
            team.PriorityData.Type = OptionTypes.Float;
            team.PriorityData.Title = FungleTranslation.TeamPriorityText;
            team.PriorityData.Increment = 1;
            team.PriorityData.ValidRange = new FloatRange(0, 500);
            team.PriorityData.FormatString = null;
            team.PriorityData.ZeroIsInfinity = false;
            team.PriorityData.SuffixType = NumberSuffixes.None;
            team.PriorityData.OptionName = FloatOptionNames.Invalid;
            plugin.BasePlugin.Log.LogInfo("Registered Team " + type.Name);
        }
    }
}
