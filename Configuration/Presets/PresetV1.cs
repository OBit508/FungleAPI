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
        }
        public void LoadConfigs()
        {
        }
        public void CleanConfigs()
        {
        }
        public override string ToString()
        {
            return PresetName;
        }
    }
}
