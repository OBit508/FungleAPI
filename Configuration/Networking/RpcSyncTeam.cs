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
    internal class RpcSyncTeam : AdvancedRpc<IEnumerable<ModdedTeam>>
    {
        public override void Write(MessageWriter messageWriter, IEnumerable<ModdedTeam> data)
        {
            messageWriter.WritePacked(data.Count());
            foreach (ModdedTeam moddedTeam in data)
            {
                messageWriter.WriteTeam(moddedTeam);
                messageWriter.Write(moddedTeam.TeamOptions.Compact());
                if (data.Count() > 1)
                {
                    messageWriter.WritePacked(moddedTeam.TeamOptions.ExtraOptions.Count);
                    foreach (ModdedOption moddedOption in moddedTeam.TeamOptions.ExtraOptions)
                    {
                        messageWriter.WriteOption(moddedOption);
                        messageWriter.Write(moddedOption.localValue.Value);
                    }
                }
                else
                {
                    HudManager.Instance?.Notifier?.AddSettingsChangeMessage(moddedTeam.TeamName, moddedTeam.TeamOptions.GetCount(), moddedTeam.TeamOptions.GetPriority(), moddedTeam.TeamColor, false, true);
                }
            }
        }
        public override void Handle(MessageReader messageReader)
        {
            int teamCount = messageReader.ReadPackedInt32();
            for (int i = 0; i < teamCount; i++)
            {
                ModdedTeam moddedTeam = messageReader.ReadTeam();
                moddedTeam.TeamOptions.Decompact(messageReader.ReadString(), false);
                if (teamCount > 1)
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
                    HudManager.Instance?.Notifier?.AddSettingsChangeMessage(moddedTeam.TeamName, moddedTeam.TeamOptions.GetCount(), moddedTeam.TeamOptions.GetPriority(), moddedTeam.TeamColor, true, true);
                }
            }
        }
    }
}
