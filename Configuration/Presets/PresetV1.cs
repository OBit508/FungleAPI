using FungleAPI.Configuration.Attributes;
using FungleAPI.Configuration.Helpers;
using FungleAPI.PluginLoading;
using Hazel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace FungleAPI.Configuration.Presets
{
    [Serializable]
    public class PresetV1
    {
        [JsonPropertyName("PresetName")]
        public string PresetName { get; set; } = "";
        [JsonPropertyName("Configs")]
        public List<string> Configs { get; set; } = new List<string>();
        [JsonPropertyName("ConfigValues")]
        public List<string> ConfigValues { get; set; } = new List<string>();
        [JsonPropertyName("RoleCountsAndChances")]
        public List<string> RoleCountsAndChances { get; set; } = new List<string>();
        [JsonPropertyName("RoleCounts")]
        public List<int> RoleCounts { get; set; } = new List<int>();
        [JsonPropertyName("RoleChances")]
        public List<int> RoleChances { get; set; } = new List<int>();
        [JsonPropertyName("TeamCountsAndPrioritys")]
        public List<string> TeamCountsAndPrioritys { get; set; } = new List<string>();
        [JsonPropertyName("TeamCounts")]
        public List<int> TeamCounts { get; set; } = new List<int>();
        [JsonPropertyName("TeamPriorityes")]
        public List<int> TeamPriorityes { get; set; } = new List<int>();
        [JsonPropertyName("Empty")]
        public bool Empty { get; set; }
        [JsonIgnore]
        public ModPlugin Plugin;
        [JsonIgnore]
        public string Path;
        public void SaveConfigs(string presetName)
        {
            PresetName = presetName;
            Plugin.Options.ForEach(new Action<ModdedOption>(delegate (ModdedOption option)
            {
                Configs.Add(option.FullConfigName);
                ConfigValues.Add(option.localValue.Value);
            }));
            Plugin.RoleCountsAndChances.ForEach(new Action<RoleCountAndChance>(delegate (RoleCountAndChance roleCountAndChance)
            {
                RoleCountsAndChances.Add(roleCountAndChance.Name);
                RoleCounts.Add(roleCountAndChance.localCount.Value);
                RoleChances.Add(roleCountAndChance.localChance.Value);
            }));
            Plugin.TeamCountAndPriorities.ForEach(new Action<TeamCountAndPriority>(delegate (TeamCountAndPriority teamCountAndPriority)
            {
                TeamCountsAndPrioritys.Add(teamCountAndPriority.Name);
                TeamCounts.Add(teamCountAndPriority.localCount.Value);
                TeamPriorityes.Add(teamCountAndPriority.localPriority.Value);
            }));
            Empty = false;
            if (this != Plugin.PluginPreset.GetDefault())
            {
                File.WriteAllText(Path, JsonSerializer.Serialize(this));
            }
        }
        public void LoadConfigs()
        {
            for (int i = 0; i < Configs.Count; i++)
            {
                ModdedOption option = Plugin.Options.FirstOrDefault(o => o.FullConfigName == Configs[i]);
                if (option != null)
                {
                    option.SetValue(ConfigValues[i]);
                }
            }
            for (int i = 0; i < RoleCountsAndChances.Count; i++)
            {
                RoleCountAndChance roleCountAndChance = Plugin.RoleCountsAndChances.FirstOrDefault(o => o.Name == RoleCountsAndChances[i]);
                if (roleCountAndChance != null)
                {
                    roleCountAndChance.SetCount(RoleCounts[i]);
                    roleCountAndChance.SetChance(RoleChances[i]);
                }
            }
            for (int i = 0; i < TeamCountsAndPrioritys.Count; i++)
            {
                TeamCountAndPriority teamCountAndPriority = Plugin.TeamCountAndPriorities.FirstOrDefault(o => o.Name == TeamCountsAndPrioritys[i]);
                if (teamCountAndPriority != null)
                {
                    teamCountAndPriority.SetCount(TeamCounts[i]);
                    teamCountAndPriority.SetPriority(TeamPriorityes[i]);
                }
            }
        }
        public void CleanConfigs()
        {
            PresetName = "";
            Configs.Clear();
            ConfigValues.Clear();
            RoleCountsAndChances.Clear();
            RoleCounts.Clear();
            RoleChances.Clear();
            TeamCountsAndPrioritys.Clear();
            TeamCounts.Clear();
            TeamPriorityes.Clear();
            Empty = true;
            if (this != Plugin.PluginPreset.GetDefault())
            {
                File.WriteAllText(Path, JsonSerializer.Serialize(this));
            }
        }
        public override string ToString()
        {
            return PresetName;
        }
    }
}
