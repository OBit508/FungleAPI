using FungleAPI.Base.Rpc;
using FungleAPI.Components;
using FungleAPI.Patches;
using FungleAPI.Utilities;
using Hazel;
using InnerNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;

namespace FungleAPI.Networking.RPCs
{
    public class RpcAmModded : SimpleRpc<PlayerControl>
    {
        public override void Write(PlayerControl player, MessageWriter writer)
        {
            writer.Write(player.Data.ClientId);
        }
        public override void Handle(MessageReader reader)
        {
            int clientId = reader.ReadInt32();
            LobbyWarningText.nonModdedPlayers.Remove(LobbyWarningText.nonModdedPlayers.Keys.FirstOrDefault(c => c.Id == clientId));
        }
    }
}
