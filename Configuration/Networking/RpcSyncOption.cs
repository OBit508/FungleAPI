using FungleAPI.Base.Rpc;
using FungleAPI.Configuration.Attributes;
using FungleAPI.Networking;
using Hazel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Il2CppSystem.Globalization.CultureInfo;

namespace FungleAPI.Configuration.Networking
{
    internal class RpcSyncOption : AdvancedRpc<ModdedOption>
    {
        public override void Write(MessageWriter messageWriter, ModdedOption data)
        {
            messageWriter.WriteOption(data);
            messageWriter.Write(data.localValue.Value);
            HudManager.Instance?.Notifier?.AddSettingsChangeMessage(data.Data.Title, data.localValue.Value, false);
        }
        public override void Handle(MessageReader messageReader)
        {
            ModdedOption moddedOption = messageReader.ReadOption();
            moddedOption.onlineValue = messageReader.ReadString();
            HudManager.Instance?.Notifier?.AddSettingsChangeMessage(moddedOption.Data.Title, moddedOption.localValue.Value, true);
        }
    }
}
