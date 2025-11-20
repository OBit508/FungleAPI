using AmongUs.GameOptions;
using Epic.OnlineServices;
using FungleAPI.Configuration;
using FungleAPI.Configuration.Attributes;
using FungleAPI.Configuration.Helpers;
using FungleAPI.Configuration.Presets;
using FungleAPI.PluginLoading;
using FungleAPI.Role;
using FungleAPI.Utilities;
using Hazel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using xCloud;
using static Il2CppSystem.Linq.Expressions.Interpreter.CastInstruction.CastInstructionNoT;

namespace FungleAPI.Networking.RPCs
{
    public class RpcUpdatePreset : CustomRpc<PresetV1>
    {
        public override void Write(MessageWriter writer, PresetV1 value)
        {
            writer.Write(File.ReadAllText(value.Path));
            HudManager.Instance.Notifier.AddSettingsChangeMessage(StringNames.ModeLabel, value.ToString(), false, RoleTypes.Crewmate);
        }
        public override void Handle(MessageReader reader)
        {
            PresetV1 preset = JsonSerializer.Deserialize<PresetV1>(reader.ReadString());
            for (int i = 0; i < preset.Configs.Count; i++)
            {
                ModdedOption option = ConfigurationManager.Options.FirstOrDefault(o => o.FullConfigName == preset.Configs[i]);
                if (option != null)
                {
                    option.SetValue(preset.ConfigValues[i]);
                }
            }
            for (int i = 0; i < preset.RoleCountsAndChances.Count; i++)
            {
                RoleCountAndChance roleCountAndChance = ConfigurationManager.RoleCountsAndChances.FirstOrDefault(o => o.Name == preset.RoleCountsAndChances[i]);
                if (roleCountAndChance != null)
                {
                    roleCountAndChance.SetCount(preset.RoleCounts[i]);
                    roleCountAndChance.SetChance(preset.RoleChances[i]);
                }
            }
            for (int i = 0; i < preset.TeamCountsAndPrioritys.Count; i++)
            {
                TeamCountAndPriority teamCountAndPriority = ConfigurationManager.TeamCountAndPriorities.FirstOrDefault(o => o.Name == preset.TeamCountsAndPrioritys[i]);
                if (teamCountAndPriority != null)
                {
                    teamCountAndPriority.SetCount(preset.TeamCounts[i]);
                    teamCountAndPriority.SetPriority(preset.TeamPriorityes[i]);
                }
            }
            HudManager.Instance.Notifier.AddSettingsChangeMessage(StringNames.ModeLabel, preset.PresetName, false, RoleTypes.Crewmate);
        }
    }
}
