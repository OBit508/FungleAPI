using FungleAPI.MCIPatches;
using FungleAPI.Patches;
using Hazel;
using InnerNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FungleAPI.Rpc
{
    public class CustomRpc<DataT> : RpcHelper
    {
        public void Send(DataT data, uint NetId, SendOption sendOption = SendOption.Reliable, int targetClientId = -1)
        {
            if (MCIUtils.GetClient(targetClientId) == null)
            {
                MessageWriter writer = AmongUsClient.Instance.StartRpcImmediately(NetId, byte.MaxValue, sendOption, targetClientId);
                writer.Write(false);
                writer.Write(ModPlugin.GetModPlugin(GetType().Assembly).ModName);
                writer.Write(GetType().FullName);
                writer.StartMessage(0);
                Write(writer, data);
                writer.EndMessage();
                AmongUsClient.Instance.FinishRpcImmediately(writer);
            }
        }
        public virtual void Write(MessageWriter writer, DataT value)
        {
        }
    }
}
