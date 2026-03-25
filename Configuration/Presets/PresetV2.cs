using FungleAPI.Configuration.Attributes;
using FungleAPI.Configuration.Helpers;
using FungleAPI.PluginLoading;
using FungleAPI.Role;
using FungleAPI.Teams;
using Il2CppSystem;
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
    public class PresetV2
    {
        public string PresetName { get; set; } = "";
        public Dictionary<string, string> Options { get; set; } = new Dictionary<string, string>();
        public Dictionary<string, string> RoleOptions { get; set; } = new Dictionary<string, string>();
        public Dictionary<string, string> TeamOptions { get; set; } = new Dictionary<string, string>();
        public bool Empty { get; set; }
        [JsonIgnore]
        public ModPlugin Plugin;
        [JsonIgnore]
        public string Path;
        public void SaveConfigs(string presetName)
        {
            PresetName = presetName;
            foreach (ModdedOption moddedOption in Plugin.Options)
            {
                Options.Add(moddedOption.FullConfigName, moddedOption.localValue.Value);
            }
            foreach (ICustomRole customRole in Plugin.Roles)
            {
                RoleOptions roleOptions = customRole.RoleOptions;
                RoleOptions.Add(roleOptions.Name, roleOptions.Compact());
            }
            foreach (ModdedTeam moddedTeam in Plugin.Teams)
            {
                TeamOptions teamOptions = moddedTeam.TeamOptions;
                TeamOptions.Add(teamOptions.Name, teamOptions.Compact());
            }
            Empty = false;
            if (this != Plugin.PluginPreset.GetDefault())
            {
                File.WriteAllText(Path, JsonSerializer.Serialize(this));
            }
        }
        public void LoadConfigs(bool local = true)
        {
            foreach (KeyValuePair<string, string> option in Options)
            {
                ModdedOption moddedOption = Plugin.Options.FirstOrDefault(o => o.FullConfigName == option.Key);
                if (moddedOption != null)
                {
                    if (local)
                    {
                        moddedOption.localValue.Value = option.Value;
                    }
                    else
                    {
                        moddedOption.onlineValue = option.Value;
                    }
                }
            }
            foreach (KeyValuePair<string, string> roleOption in RoleOptions)
            {
                RoleOptions roleOptions = Plugin.RoleOptions.FirstOrDefault(o => o.Name == roleOption.Key);
                if (roleOptions != null)
                {
                    roleOptions.Decompact(roleOption.Value, local);
                }
            }
            foreach (KeyValuePair<string, string> teamOption in TeamOptions)
            {
                TeamOptions teamOptions = Plugin.TeamOptions.FirstOrDefault(o => o.Name == teamOption.Key);
                if (teamOptions != null)
                {
                    teamOptions.Decompact(teamOption.Value, local);
                }
            }
        }
        public void CleanConfigs()
        {
            PresetName = "";
            Options.Clear();
            RoleOptions.Clear();
            TeamOptions.Clear();
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
