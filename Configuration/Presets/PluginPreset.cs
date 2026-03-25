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
    /// <summary>
    /// The class that stores the corresponding plugin presets
    /// </summary>
    public class PluginPreset
    {
        public ConfigEntry<string> CurrentPresetVersion;
        public ModPlugin Plugin;
        public List<PresetV2> Presets = new List<PresetV2>();
        private PresetV2 Default;
        public void Initialize()
        {
            string path = Path.Combine(ConfigurationManager.FunglePath, Plugin.RealName + " - " + Plugin.LocalMod.GUID);
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            for (int i = 1; i <= 5; i++)
            {
                PresetV2 preset;
                string p = Path.Combine(path, "Preset " + i.ToString() + ".json");
                if (!File.Exists(p))
                {
                    preset = new PresetV2() { Empty = true };
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
                        preset = JsonSerializer.Deserialize<PresetV2>(File.ReadAllText(p));
                    }
                }
                preset.Path = p;
                preset.Plugin = Plugin;
                Presets.Add(preset);
            }
        }
        public PresetV2 GetDefault()
        {
            if (Default == null)
            {
                Default = new PresetV2();
                Default.Plugin = Plugin;
                Default.SaveConfigs("Default");
            }
            return Default;
        }
        public PresetV2 TryPort(string path)
        {
            return new PresetV2() { Empty = true };
        }
    }
}
