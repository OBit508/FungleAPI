
using FungleAPI.Attributes;
using FungleAPI.ModCompatibility;
using FungleAPI.Networking;
using FungleAPI.Patches;
using FungleAPI.PluginLoading;
using Hazel;
using InnerNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FungleAPI.Networking.RPCs
{
    [FungleIgnore]
    public class CustomRpc<DataT> : RpcHelper
    {
        public void Send(DataT data, uint NetId, SendOption sendOption = SendOption.Reliable, int targetClientId = -1)
        {
            MessageWriter writer = AmongUsClient.Instance.StartRpcImmediately(NetId, byte.MaxValue, sendOption, targetClientId);
            writer.StartMessage(0);
            writer.Write(ModPluginManager.GetModPlugin(GetType().Assembly).ModName);
            writer.Write(GetType().FullName);
            Write(writer, data);
            writer.EndMessage();
            AmongUsClient.Instance.FinishRpcImmediately(writer);
        }
        public virtual void Write(MessageWriter writer, DataT value)
        {
        }
    }
    [FungleIgnore]
    public class CustomRpc : RpcHelper
    {
        public void Send(uint NetId, SendOption sendOption = SendOption.Reliable, int targetClientId = -1)
        {
            MessageWriter writer = AmongUsClient.Instance.StartRpcImmediately(NetId, byte.MaxValue, sendOption, targetClientId);
            writer.StartMessage(0);
            writer.Write(ModPluginManager.GetModPlugin(GetType().Assembly).ModName);
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
