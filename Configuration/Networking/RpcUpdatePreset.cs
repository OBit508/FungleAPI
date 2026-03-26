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
    internal class RpcUpdatePreset : AdvancedRpc<PresetV2>
    {
        public override void Write(MessageWriter messageWriter, PresetV2 value)
        {
            messageWriter.Write(JsonSerializer.Serialize(value));
            messageWriter.WritePlugin(value.Plugin);
            HudManager.Instance?.Notifier?.AddSettingsChangeMessage(StringNames.ModeLabel, $"{value.Plugin.ModName}:{value.PresetName}", false, RoleTypes.Crewmate);
        }
        public override void Handle(MessageReader messageReader)
        {
            PresetV2 preset = JsonSerializer.Deserialize<PresetV2>(messageReader.ReadString());
            preset.Plugin = messageReader.ReadPlugin();
            preset.LoadConfigs(false);
            HudManager.Instance?.Notifier?.AddSettingsChangeMessage(StringNames.ModeLabel, $"{preset.Plugin.ModName}:{preset.PresetName}", true, RoleTypes.Crewmate);
        }
    }
}
