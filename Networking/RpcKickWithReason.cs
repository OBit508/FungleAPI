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
    internal class RpcKickWithReason : AdvancedRpc<string>
    {
        public override bool CanAcceptRPCsWithoutInnerNetObject => true;
        public override void Write(MessageWriter messageWriter, string data)
        {
            messageWriter.Write(data);
        }
        public override void Handle(MessageReader messageReader)
        {
            string reason = messageReader.ReadString();
            HandShakeManager.DisconnectWithReason(reason);
        }
    }
}
