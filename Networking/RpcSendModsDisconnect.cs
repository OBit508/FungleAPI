using FungleAPI.Base.Rpc;
using Hazel;
using InnerNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FungleAPI.Networking
{
    internal class RpcSendModsDisconnect : AdvancedRpc<ModsDisconnectData>
    {
        public override bool RequiresNetObject => false;
        public override void Write(MessageWriter messageWriter, ModsDisconnectData data)
        {
            messageWriter.Write(data.MissingMods != null);
            if (data.MissingMods != null)
            {
                messageWriter.Write(data.MissingMods);
            }
            messageWriter.Write(data.ExtraMods != null);
            if (data.ExtraMods != null)
            {
                messageWriter.Write(data.ExtraMods);
            }
        }
        public override void Handle(MessageReader messageReader)
        {
            HandShakeManager.DisconnectData = new ModsDisconnectData(messageReader);
        }
    }
}
