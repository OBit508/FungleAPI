using BepInEx.Configuration;
using FungleAPI.Configuration.Attributes;
using FungleAPI.Configuration.Helpers;
using FungleAPI.PluginLoading;
using FungleAPI.Role;
using FungleAPI.Teams;
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
            string path = FileManager.GetFolder(Plugin, FolderType.Presets);
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
                Default = new PresetV2() { PresetName = "Default" };
                Default.Plugin = Plugin;
                foreach (ModdedOption moddedOption in Plugin.Options)
                {
                    Default.Options[moddedOption.FullConfigName] = moddedOption.localValue.DefaultValue.ToString();
                }
                foreach (ICustomRole role in Plugin.Roles)
                {
                    RoleOptions roleOptions = role.RoleOptions;
                    using (MemoryStream ms = new MemoryStream())
                    {
                        using (BinaryWriter bw = new BinaryWriter(ms))
                        {
                            bw.Write((int)roleOptions.localCount.DefaultValue);
                            bw.Write((int)roleOptions.localChance.DefaultValue);
                            Default.RoleOptions[roleOptions.Name] = Encoding.UTF8.GetString(ms.ToArray());
                        }
                    }
                }
                foreach (ModdedTeam team in Plugin.Teams)
                {
                    TeamOptions teamOptions = team.TeamOptions;
                    using (MemoryStream ms = new MemoryStream())
                    {
                        using (BinaryWriter bw = new BinaryWriter(ms))
                        {
                            bw.Write((int)teamOptions.localCount.DefaultValue);
                            bw.Write((int)teamOptions.localPriority.DefaultValue);
                            Default.TeamOptions[teamOptions.Name] = Encoding.UTF8.GetString(ms.ToArray());
                        }
                    }
                }
            }
            return Default;
        }
        public PresetV2 TryPort(string path)
        {
            return new PresetV2() { Empty = true };
        }
    }
}
