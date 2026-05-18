using FungleAPI.Base.Rpc;
using FungleAPI.Networking;
using FungleAPI.Role;
using FungleAPI.Teams;
using Hazel;
using InnerNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FungleAPI.GameOptions.Networking
{
    internal class RpcSyncEverything : SimpleRpc
    {
        public static bool Synced;
        public override void Write(MessageWriter messageWriter)
        {
            messageWriter.WritePacked(OptionManager.AllOptions.Count);
            foreach (IModdedOption moddedOption in OptionManager.AllOptions.Values)
            {
                messageWriter.WriteOption(moddedOption);
                moddedOption.Serialize(messageWriter);
            }

            RpcSyncRole rpcSyncRole = Rpc<RpcSyncRole>.Instance;
            RpcSyncTeam rpcSyncTeam = Rpc<RpcSyncTeam>.Instance;

            messageWriter.WritePacked(CustomRoleManager.AllCustomRoles.Count);
            foreach (ICustomRole customRole in CustomRoleManager.AllCustomRoles)
            {
                rpcSyncRole.Write(messageWriter, customRole);
            }
            messageWriter.WritePacked(ModdedTeamManager.Teams.Count);
            foreach (ModdedTeam moddedTeam in ModdedTeamManager.Teams.Values)
            {
                rpcSyncTeam.Write(messageWriter, moddedTeam);
            }
        }
        public override void Handle(MessageReader messageReader)
        {
            Synced = false;
            int optionCount = messageReader.ReadPackedInt32();
            for (int i = 0; i < optionCount; i++)
            {
                IModdedOption moddedOption = messageReader.ReadOption();
                moddedOption.Deserialize(messageReader);
            }

            RpcSyncRole rpcSyncRole = Rpc<RpcSyncRole>.Instance;
            RpcSyncTeam rpcSyncTeam = Rpc<RpcSyncTeam>.Instance;

            int roleCount = messageReader.ReadPackedInt32();
            for (int i = 0; i < roleCount; i++)
            {
                rpcSyncRole.Handle(messageReader);
            }
            int teamCount = messageReader.ReadPackedInt32();
            for (int i = 0; i < teamCount; i++)
            {
                rpcSyncTeam.Handle(messageReader);
            }
            Synced = true;
        }
    }
}
