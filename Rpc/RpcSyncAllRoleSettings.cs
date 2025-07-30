using AmongUs.GameOptions;
using FungleAPI.Configuration;
using FungleAPI.Role;
using FungleAPI.Roles;
using Hazel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FungleAPI.Rpc
{
    public class RpcSyncAllRoleSettings : CustomRpc<string>
    {
        public override void Read(MessageReader reader)
        {
            int count = reader.ReadInt32();
            for (int i = 0; i < count; i++)
            {
                string fullConfigName = reader.ReadString();
                string configValue = reader.ReadString();
                foreach (CustomConfig config in ConfigurationManager.Configs.Values)
                {
                    if (config.FullConfigName == fullConfigName)
                    {
                        config.SetValue(configValue);
                        break;
                    }
                }
            }
            bool stringNull = reader.ReadBoolean();
            if (!stringNull)
            {
                string str = reader.ReadString();
                Handle(str);
            }
        }
        public override void Handle(string value)
        {
            HudManager.Instance.Notifier.SettingsChangeMessageLogic(StringNames.None, value, !AmongUsClient.Instance.AmHost);
        }
        public override void Write(MessageWriter writer, string value)
        {
            writer.Write(ConfigurationManager.Configs.Values.Count);
            foreach (CustomConfig config in ConfigurationManager.Configs.Values)
            {
                writer.Write(config.FullConfigName);
                writer.Write(config.GetValue());
            }
            writer.Write(value == null);
            if (value != null)
            {
                writer.Write(value);
            }
        }
    }
}
