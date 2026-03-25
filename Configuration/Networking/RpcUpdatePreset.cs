using AmongUs.GameOptions;
using Epic.OnlineServices;
using FungleAPI.Base.Rpc;
using FungleAPI.Configuration;
using FungleAPI.Configuration.Attributes;
using FungleAPI.Configuration.Helpers;
using FungleAPI.Configuration.Presets;
using FungleAPI.Networking;
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

namespace FungleAPI.Configuration.Networking
{
    /// <summary>
    /// Rpc that change the current host preset
    /// </summary>
    public class RpcUpdatePreset : AdvancedRpc<PresetV2>
    {
        public override void Write(MessageWriter writer, PresetV2 value)
        {
            writer.Write(JsonSerializer.Serialize(value));
            writer.WritePlugin(value.Plugin);
            HudManager.Instance.Notifier.AddSettingsChangeMessage(StringNames.ModeLabel, value.ToString(), false, RoleTypes.Crewmate);
        }
        public override void Handle(MessageReader reader)
        {
            PresetV2 preset = JsonSerializer.Deserialize<PresetV2>(reader.ReadString());
            preset.Plugin = reader.ReadPlugin();
            preset.LoadConfigs(false);
            HudManager.Instance.Notifier.AddSettingsChangeMessage(StringNames.ModeLabel, preset.PresetName, false, RoleTypes.Crewmate);
        }
    }
}
