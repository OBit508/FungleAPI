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
    public class RpcSyncAllRoleSettings : CustomRpc<object>
    {
        public override void Read(MessageReader reader)
        {
            int count = reader.ReadInt32();
            for (int i = 0; i < count; i++)
            {
                ICustomRole role = CustomRoleManager.GetRole((RoleTypes)reader.ReadInt32());
                foreach (Config config in role.CachedConfiguration.Configs)
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
        }
        public override void Write(MessageWriter writer, object value)
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
                foreach (Config config in role.CachedConfiguration.Configs)
                {
                    if (config is NumConfig n)
                    {
                        writer.Write(n.ConfigEntry.Value);
                    }
                    else if (config is BoolConfig b)
                    {
                        writer.Write(b.ConfigEntry.Value);
                    }
                    else if (config is EnumConfig e)
                    {
                        writer.Write(e.ConfigEntry.Value);
                    }
                }
            }
        }
    }
}
