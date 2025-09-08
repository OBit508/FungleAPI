using AmongUs.GameOptions;
using FungleAPI.Configuration;
using FungleAPI.Role;
using FungleAPI.Networking;
using Hazel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FungleAPI.Networking.RPCs
{
    public class RpcSyncSeetings : CustomRpc<(ICustomRole role, string text, bool playSound, bool handlePlaySound)>
    {
        public override void Handle(MessageReader reader)
        {
            ICustomRole role = CustomRoleManager.GetRole((RoleTypes)reader.ReadByte());
            role.Configuration.onlineCount = reader.ReadInt32();
            role.Configuration.onlineChance = reader.ReadInt32();
            int count = reader.ReadInt32();
            for (int i = 0; i < count; i++)
            {
                ModdedOption config = reader.ReadConfig();
                if (config != null)
                {
                    config.SetValue(reader.ReadString());
                }
            }
            string text = reader.ReadString();
            bool playSound = reader.ReadBoolean();
            HudManager.Instance.Notifier.SettingsChangeMessageLogic(StringNames.None, text, playSound);
        }
        public override void Write(MessageWriter writer, (ICustomRole role, string text, bool playSound, bool handlePlaySound) value)
        {
            writer.Write((byte)value.role.Role);
            writer.Write(value.role.RoleCount);
            writer.Write(value.role.RoleChance);
            writer.Write(value.role.Configuration.Configs.Count);
            for (int i = 0; i < value.role.Configuration.Configs.Count; i++)
            {
                ModdedOption config = value.role.Configuration.Configs[i];
                writer.WriteConfig(config);
                writer.Write(config.GetValue());
            }
            writer.Write(value.text);
            writer.Write(value.handlePlaySound);
            HudManager.Instance.Notifier.SettingsChangeMessageLogic(StringNames.None, value.text, value.playSound);
        }
    }
}
