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
    public class RpcSyncSeetings : CustomRpc<ICustomRole>
    {
        public override void Handle(MessageReader reader)
        {
            ICustomRole role = CustomRoleManager.GetRole((RoleTypes)reader.ReadByte());
            role.Configuration.onlineCount = reader.ReadInt32();
            role.Configuration.onlineChance = reader.ReadInt32();
            int count = reader.ReadInt32();
            for (int i = 0; i < count; i++)
            {
                CustomOption config = reader.ReadConfig();
                if (config != null)
                {
                    config.SetValue(reader.ReadString());
                }
            }
        }
        public override void Write(MessageWriter writer, ICustomRole value)
        {
            writer.Write((byte)value.Role);
            writer.Write(value.RoleCount);
            writer.Write(value.RoleChance);
            writer.Write(value.Configuration.Configs.Count);
            for (int i = 0; i < value.Configuration.Configs.Count; i++)
            {
                CustomOption config = value.Configuration.Configs[i];
                writer.WriteConfig(config);
                writer.Write(config.GetValue());
            }
        }
    }
}
