using BepInEx.Configuration;
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
        public PresetV1 TryPort(string path)
        {
            return new PresetV1() { Empty = true };
        }
    }
}
