using FungleAPI.Base.Rpc;
using FungleAPI.Configuration.Attributes;
using FungleAPI.Networking;
using FungleAPI.Role;
using FungleAPI.Teams;
using Hazel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FungleAPI.Utilities;

namespace FungleAPI.Configuration.Networking
{
    internal class RpcSyncRole : AdvancedRpc<IEnumerable<ICustomRole>>
    {
        public override void Write(MessageWriter messageWriter, IEnumerable<ICustomRole> data)
        {
            messageWriter.WritePacked(data.Count());
            foreach (ICustomRole customRole in data)
            {
                messageWriter.WriteRole(customRole as RoleBehaviour);
                messageWriter.Write(customRole.RoleOptions.Compact());
                if (data.Count() > 1)
                {
                    messageWriter.WritePacked(customRole.RoleOptions.Options.Count);
                    foreach (ModdedOption moddedOption in customRole.RoleOptions.Options)
                    {
                        messageWriter.WriteOption(moddedOption);
                        messageWriter.Write(moddedOption.localValue.Value);
                    }
                }
                else
                {
                    HudManager.Instance?.Notifier?.AddSettingsChangeMessage(customRole.RoleName, customRole.RoleOptions.GetCount(), customRole.RoleOptions.GetChance(), customRole.RoleColor, false);
                }
            }
        }
        public override void Handle(MessageReader messageReader)
        {
            int roleCount = messageReader.ReadPackedInt32();
            for (int i = 0; i < roleCount; i++)
            {
                ICustomRole customRole = messageReader.ReadRole().CustomRole();
                customRole.RoleOptions.Decompact(messageReader.ReadString(), false);
                if (roleCount > 1)
                {
                    int optionCount = messageReader.ReadPackedInt32();
                    int x = 0;
                    while (x < optionCount)
                    {
                        ModdedOption moddedOption = messageReader.ReadOption();
                        moddedOption.onlineValue = messageReader.ReadString();
                        x++;
                    }
                }
                else
                {
                    HudManager.Instance?.Notifier?.AddSettingsChangeMessage(customRole.RoleName, customRole.RoleOptions.GetCount(), customRole.RoleOptions.GetChance(), customRole.RoleColor, true);
                }
            }
        }
    }
}
