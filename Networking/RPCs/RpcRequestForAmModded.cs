using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FungleAPI.Base.Rpc;
using Hazel;

namespace FungleAPI.Networking.RPCs
{
    public class RpcRequestForAmModded : SimpleRpc
    {
        public override void Handle(MessageReader messageReader)
        {
            CustomRpcManager.Instance<RpcAmModded>().Send(PlayerControl.LocalPlayer);
        }
    }
}
