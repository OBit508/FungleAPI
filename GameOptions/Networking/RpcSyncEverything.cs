using FungleAPI.Base.Rpc;
using FungleAPI.GModes;
using FungleAPI.Networking;
using FungleAPI.Role;
using FungleAPI.Teams;
using FungleAPI.Translation;
using FungleAPI.Utilities;
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
        public override bool RequiresNetObject => false;
        public static bool UnSynced;
        public override void Write(MessageWriter messageWriter)
        {
            messageWriter.WritePacked(OptionManager.AllOptions.Count);
            foreach (IModdedOption moddedOption in OptionManager.AllOptions.Values)
            {
                messageWriter.WriteOption(moddedOption);
                moddedOption.Serialize(messageWriter);
            }

            RpcSyncGamemode rpcSyncGamemode = Rpc<RpcSyncGamemode>.Instance;
            RpcSyncRole rpcSyncRole = Rpc<RpcSyncRole>.Instance;
            RpcSyncTeam rpcSyncTeam = Rpc<RpcSyncTeam>.Instance;

            rpcSyncGamemode.Write(messageWriter);
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
            try
            {
                UnSynced = true;
                int optionCount = messageReader.ReadPackedInt32();
                for (int i = 0; i < optionCount; i++)
                {
                    IModdedOption moddedOption = messageReader.ReadOption();
                    moddedOption.Deserialize(messageReader);
                }

                RpcSyncGamemode rpcSyncGamemode = Rpc<RpcSyncGamemode>.Instance;
                RpcSyncRole rpcSyncRole = Rpc<RpcSyncRole>.Instance;
                RpcSyncTeam rpcSyncTeam = Rpc<RpcSyncTeam>.Instance;

                rpcSyncGamemode.Handle(messageReader);
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
                UnSynced = false;
            }
            catch (Exception ex)
            {
                HandShakeManager.DisconnectWithReason(FungleTranslation.FailedToSync.GetString() + ex.Message);
            }
        }
    }
}
