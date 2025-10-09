
using FungleAPI.Patches;
using FungleAPI.Networking;
using Hazel;
using InnerNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FungleAPI.ModCompatibility;

namespace FungleAPI.Networking.RPCs
{
    public class CustomRpc<DataT> : RpcHelper
    {
        public void Send(DataT data, uint NetId, SendOption sendOption = SendOption.Reliable, int targetClientId = -1)
        {
            MessageWriter writer = AmongUsClient.Instance.StartRpcImmediately(NetId, byte.MaxValue, sendOption, targetClientId);
            writer.StartMessage(0);
            writer.Write(ModPlugin.GetModPlugin(GetType().Assembly).ModName);
            writer.Write(GetType().FullName);
            Write(writer, data);
            writer.EndMessage();
            AmongUsClient.Instance.FinishRpcImmediately(writer);
        }
        public virtual void Write(MessageWriter writer, DataT value)
        {
        }
    }
    public class CustomRpc : RpcHelper
    {
        public void Send(uint NetId, SendOption sendOption = SendOption.Reliable, int targetClientId = -1)
        {
            MessageWriter writer = AmongUsClient.Instance.StartRpcImmediately(NetId, byte.MaxValue, sendOption, targetClientId);
            writer.StartMessage(0);
            writer.Write(ModPlugin.GetModPlugin(GetType().Assembly).ModName);
            writer.Write(GetType().FullName);
            Write(writer);
            writer.EndMessage();
            AmongUsClient.Instance.FinishRpcImmediately(writer);
        }
        public virtual void Write(MessageWriter writer)
        {
        }
    }
}
