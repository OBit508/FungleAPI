using Hazel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FungleAPI.Networking.RPCs
{
    public class RpcRequestMods : CustomRpc
    {
        public override void Write(MessageWriter writer)
        {
            writer.Write(AmongUsClient.Instance.ClientId);
        }
        public override void Handle(MessageReader reader)
        {
            int clientId = reader.ReadInt32();
            if (clientId != -1 && AmongUsClient.Instance.ClientId != -1 && PlayerControl.LocalPlayer != null)
            {
                CustomRpcManager.Instance<RpcSendMods>().Send(AmongUsClient.Instance.GetClient(AmongUsClient.Instance.ClientId), PlayerControl.LocalPlayer.NetId, SendOption.Reliable, clientId);
            }
        }
    }
}
