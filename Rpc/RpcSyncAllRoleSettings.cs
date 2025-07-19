using AmongUs.GameOptions;
using FungleAPI.Role;
using FungleAPI.Role.Configuration;
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
                ICustomRole role = CustomRoleManager.GetRole((RoleTypes)reader.ReadInt32());
                foreach (CustomConfig config in role.CachedConfiguration.Configs)
                {
                    config.SetValue(reader.ReadString());
                }
            }
            bool stringNull = reader.ReadBoolean();
            if (!stringNull)
            {
                string str = reader.ReadString();
                HudManager.Instance.Notifier.SettingsChangeMessageLogic(StringNames.None, str, !AmongUsClient.Instance.AmHost);
            }
        }
        public override void Write(MessageWriter writer, string value)
        {
            List<ICustomRole> roles = new List<ICustomRole>();
            foreach (RoleBehaviour role in CustomRoleManager.AllRoles)
            {
                if (role.CustomRole() != null)
                {
                    roles.Add(role.CustomRole());
                }
            }
            writer.Write(roles.Count);
            foreach (ICustomRole role in roles)
            {
                writer.Write((int)role.Role);
                foreach (CustomConfig config in role.CachedConfiguration.Configs)
                {
                    writer.Write(config.GetValue());
                }
            }
            writer.Write(value == null);
            if (value != null)
            {
                writer.Write(value);
                HudManager.Instance.Notifier.SettingsChangeMessageLogic(StringNames.None, value, !AmongUsClient.Instance.AmHost);
            }
        }
    }
}
