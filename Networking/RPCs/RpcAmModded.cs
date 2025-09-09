using FungleAPI.Components;
using FungleAPI.Patches;
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
    public class RpcAmModded : CustomRpc<PlayerControl>
    {
        public override void Write(MessageWriter writer, PlayerControl value)
        {
            writer.Write(value.Data.ClientId);
        }
        public override void Handle(MessageReader reader)
        {
            int clientId = reader.ReadInt32();
            foreach (ClientData client in LobbyWarningText.nonModdedPlayers.Keys)
            {
                if (client.Id == clientId)
                {
                    LobbyWarningText.nonModdedPlayers.Remove(client);
                    return;
                }
            }
        }
    }
}
