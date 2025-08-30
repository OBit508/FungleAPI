using FungleAPI.Patches;
using Hazel;
using InnerNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FungleAPI.Networking.RPCs
{
    internal class RpcAmModdedClient : CustomRpc<int>
    {
        public override void Handle(MessageReader reader)
        {
            int clientId = reader.ReadInt32();
            foreach (ClientData client in AmongUsClientPatch.NonModdedClients)
            {
                if (client.Id == clientId)
                {
                    AmongUsClientPatch.NonModdedClients.Remove(client);
                    return;
                }
            }
        }
        public override void Write(MessageWriter writer, int value)
        {
            writer.Write(value);
        }
    }
}
