using AmongUs.GameOptions;
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
        internal static int LastTeamId = int.MinValue;
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
        public static void RegisterTeam(Type type, ModPlugin plugin)
        {
            LastTeamId++;
            ModdedTeam team = (ModdedTeam)Activator.CreateInstance(type);
            team.TeamId = LastTeamId;
            plugin.Teams.Add(team);
            Teams.Add(type, team);
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
