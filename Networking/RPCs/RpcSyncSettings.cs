using AmongUs.GameOptions;
using FungleAPI.Configuration;
using FungleAPI.Configuration.Attributes;
using FungleAPI.Role;
using Hazel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FungleAPI.Networking.RPCs
{
    public class RpcSyncSettings : CustomRpc<(string text, bool playSound, bool handlePlaySound)>
    {
        public override void Handle(MessageReader reader)
        {
            int count = reader.ReadInt32();
            for (int i = 0; i < count; i++)
            {
                ModdedOption option = reader.ReadConfig();
                option.SetValue(reader.ReadString());
            }
            string text = reader.ReadString();
            bool playSound = reader.ReadBoolean();
            HudManager.Instance.Notifier.SettingsChangeMessageLogic(StringNames.None, text, playSound);
        }
        public override void Write(MessageWriter writer, (string text, bool playSound, bool handlePlaySound) value)
        {
            ModdedOption[] options = ConfigurationManager.Configs.Values.ToArray();
            writer.Write((byte)options.Count());
            for (int i = 0; i < options.Count(); i++)
            {
                writer.WriteConfig(options[i]);
                writer.Write(options[i].GetValue());
            }
            writer.Write(value.text);
            writer.Write(value.handlePlaySound);
            HudManager.Instance.Notifier.SettingsChangeMessageLogic(StringNames.None, value.text, value.playSound);
        }
    }
}
