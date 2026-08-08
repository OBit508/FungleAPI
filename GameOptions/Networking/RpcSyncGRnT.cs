using BepInEx.Unity.IL2CPP.Utils.Collections;
using FungleAPI.Api;
using FungleAPI.Base.Rpc;
using FungleAPI.GameModes;
using FungleAPI.Networking;
using FungleAPI.PluginLoading;
using FungleAPI.Role;
using FungleAPI.Teams;
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
    internal class RpcSyncGRnT : SimpleRpc<PlayerControl>
    {
        public static bool UnSynced;
        public override void Write(MessageWriter messageWriter)
        {
            UnSynced = true;

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

            UnSynced = false;
        }
        public override void Handle(PlayerControl innerNetObject, MessageReader messageReader)
        {
            if (!AntiCheatManager.CheckForCheater(innerNetObject)) return;

            try
            {
                UnSynced = true;
                RpcSyncGamemode rpcSyncGamemode = Rpc<RpcSyncGamemode>.Instance;
                RpcSyncRole rpcSyncRole = Rpc<RpcSyncRole>.Instance;
                RpcSyncTeam rpcSyncTeam = Rpc<RpcSyncTeam>.Instance;

                rpcSyncGamemode.Handle(innerNetObject, messageReader);
                int roleCount = messageReader.ReadPackedInt32();
                for (int i = 0; i < roleCount; i++)
                {
                    rpcSyncRole.Handle(innerNetObject, messageReader);
                }
                int teamCount = messageReader.ReadPackedInt32();
                for (int i = 0; i < teamCount; i++)
                {
                    rpcSyncTeam.Handle(innerNetObject, messageReader);
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
