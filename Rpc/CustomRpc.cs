using Hazel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FungleAPI.LoadMod;
using FungleAPI.Patches;

namespace FungleAPI.Rpc
{
    public class CustomRpc<DataT> : RpcHelper
    {
        public void Send(DataT data, uint NetId, SendOption sendOption = SendOption.Reliable, int targetClientId = -1)
        {
            MessageWriter writer = AmongUsClient.Instance.StartRpcImmediately(NetId, 70, sendOption, targetClientId);
            writer.Write(ModPlugin.GetModPlugin(GetType().Assembly).ModName);
            writer.Write(GetType().FullName);
            Write(writer, data);
            AmongUsClient.Instance.FinishRpcImmediately(writer);
            OnSend();
        }
        public virtual void OnSend()
        {
        }
        public virtual void Write(MessageWriter writer, DataT value)
        {
        }
    }
}
