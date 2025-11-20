using BepInEx.Configuration;
using FungleAPI.Configuration.Attributes;
using FungleAPI.Configuration.Helpers;
using FungleAPI.PluginLoading;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace FungleAPI.Configuration.Presets
{
    public class PluginPreset
    {
        public ConfigEntry<string> CurrentPresetVersion;
        public ModPlugin Plugin;
        public List<PresetV1> Presets = new List<PresetV1>();
        private PresetV1 Default;
        public void Initialize()
        {
            string path = Path.Combine(ConfigurationManager.FunglePath, Plugin.RealName + " - " + Plugin.LocalMod.GUID);
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            for (int i = 1; i <= 5; i++)
            {
                PresetV1 preset;
                string p = Path.Combine(path, "Preset " + i.ToString() + ".json");
                if (!File.Exists(p))
                {
                    preset = new PresetV1() { Empty = true };
                    File.WriteAllText(p, JsonSerializer.Serialize(preset));
                }
                else
                {
                    if (CurrentPresetVersion.Value != ConfigurationManager.CurrentVersion)
                    {
                        preset = TryPort(p);
                        File.WriteAllText(p, JsonSerializer.Serialize(preset));
                        CurrentPresetVersion.Value = ConfigurationManager.CurrentVersion;
                    }
                    else
                    {
                        preset = JsonSerializer.Deserialize<PresetV1>(File.ReadAllText(p));
                    }
                }
                preset.Path = p;
                preset.Plugin = Plugin;
                Presets.Add(preset);
            }
        }
        public PresetV1 GetDefault()
        {
            if (Default == null)
            {
                Default = new PresetV1();
                Default.Plugin = Plugin;
                Default.PresetName = "Default";
                Plugin.Options.ForEach(new Action<ModdedOption>(delegate (ModdedOption option)
                {
                    Default.Configs.Add(option.FullConfigName);
                    Default.ConfigValues.Add(option.localValue.DefaultValue.ToString());
                }));
                Plugin.RoleCountsAndChances.ForEach(new Action<RoleCountAndChance>(delegate (RoleCountAndChance roleCountAndChance)
                {
                    Default.RoleCountsAndChances.Add(roleCountAndChance.Name);
                    Default.RoleCounts.Add((int)roleCountAndChance.localCount.DefaultValue);
                    Default.RoleChances.Add((int)roleCountAndChance.localChance.DefaultValue);
                }));
                Plugin.TeamCountAndPriorities.ForEach(new Action<TeamCountAndPriority>(delegate (TeamCountAndPriority teamCountAndPriority)
                {
                    Default.TeamCountsAndPrioritys.Add(teamCountAndPriority.Name);
                    Default.TeamCounts.Add((int)teamCountAndPriority.localCount.DefaultValue);
                    Default.TeamPriorityes.Add((int)teamCountAndPriority.localPriority.DefaultValue);
                }));
                Default.Empty = false;
            }
            return Default;
        }
        public PresetV1 TryPort(string path)
        {
            return new PresetV1() { Empty = true };
        }
    }
}
