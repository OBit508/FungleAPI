using FungleAPI.AntiCheat;
using FungleAPI.Base.Rpc;
using Hazel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FungleAPI.Networking
{
    internal class RpcSendModsDisconnect : AdvancedRpc<KeyValuePair<string, string>, NetworkedPlayerInfo>
    {
        public override void Write(MessageWriter messageWriter, KeyValuePair<string, string> data)
        {
            messageWriter.Write(data.Key);
            messageWriter.Write(data.Value);
        }
        public override void Handle(NetworkedPlayerInfo innerNetObject, MessageReader messageReader)
        {
            if (innerNetObject == null) return;

            if (AntiCheatManager.Active && !innerNetObject.IsHost())
            {
                AntiCheatManager.CheaterFinded(innerNetObject.ClientId);

                return;
            }

            HandShakeManager.MissingMods = messageReader.ReadString();
            HandShakeManager.ExtraMods = messageReader.ReadString();
        }
    }
}
