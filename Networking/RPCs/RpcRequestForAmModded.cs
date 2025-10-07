using Hazel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FungleAPI.Networking.RPCs
{
    public class RpcRequestForAmModded : CustomRpc
    {
        public override void Handle(MessageReader reader)
        {
            if (PlayerControl.LocalPlayer != null && PlayerControl.LocalPlayer.Data != null)
            {
                CustomRpcManager.Instance<RpcAmModded>().Send(PlayerControl.LocalPlayer, PlayerControl.LocalPlayer.NetId, SendOption.Reliable, AmongUsClient.Instance.HostId);
            }
        }
    }
}
