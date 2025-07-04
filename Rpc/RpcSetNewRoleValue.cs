using AmongUs.GameOptions;
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
    public class RpcSetNewRoleValue : CustomRpc<(ICustomRole role, Config config, object value, string notificationText)>
    {
        public override void Read(MessageReader reader)
        {
            ICustomRole role = CustomRoleManager.GetRole((RoleTypes)reader.ReadInt32());
            string configName = reader.ReadString();
            string notificationText = reader.ReadString();
            foreach (Config config in role.CachedConfiguration.Configs)
            {
                if (config.ConfigName == configName)
                {
                    if (config is NumConfig n)
                    {
                        n.ConfigEntry.Value = reader.ReadSingle();
                    }
                    else if (config is BoolConfig b)
                    {
                        b.ConfigEntry.Value = reader.ReadBoolean();
                    }
                    else if (config is EnumConfig e)
                    {
                        e.ConfigEntry.Value = reader.ReadString();
                    }
                }
            }
            if (!notificationText.IsNullOrWhiteSpace())
            {
                HudManager.Instance.Notifier.SettingsChangeMessageLogic(StringNames.None, notificationText, true);
            }
        }
        public override void Write(MessageWriter writer, (ICustomRole role, Config config, object value, string notificationText) value)
        {
            writer.Write((int)value.role.Role);
            writer.Write(value.config.ConfigName);
            writer.Write(value.notificationText);
            if (value.config is NumConfig n)
            {
                writer.Write((int)value.value);
                n.ConfigEntry.Value = (int)value.value;
            }
            else if (value.config is BoolConfig b)
            {
                writer.Write((bool)value.value);
                b.ConfigEntry.Value = (bool)value.value;
            }
            else if (value.config is EnumConfig e)
            {
                writer.Write((string)value.value);
                e.ConfigEntry.Value = (string)value.value;
            }
            if (!value.notificationText.IsNullOrWhiteSpace())
            {
                HudManager.Instance.Notifier.SettingsChangeMessageLogic(StringNames.None, value.notificationText, true);
            }
        }
    }
}
