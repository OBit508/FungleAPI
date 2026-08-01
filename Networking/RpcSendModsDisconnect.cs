using FungleAPI.Base.Rpc;
using Hazel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FungleAPI.Networking
{
    internal class RpcSendModsDisconnect : AdvancedRpc<KeyValuePair<string, string>, PlayerControl>
    {
        public override void Write(MessageWriter messageWriter, KeyValuePair<string, string> data)
        {
            messageWriter.Write(data.Key);
            messageWriter.Write(data.Value);
        }
        public override void Handle(PlayerControl innerNetObject, MessageReader messageReader)
        {
            if (innerNetObject.OwnerId != AmongUsClient.Instance.HostId) return;

            HandShakeManager.MissingMods = messageReader.ReadString();
            HandShakeManager.ExtraMods = messageReader.ReadString();
        }
    }
}
