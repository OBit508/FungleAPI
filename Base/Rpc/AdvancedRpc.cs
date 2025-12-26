using FungleAPI.Attributes;
using FungleAPI.Networking;
using Hazel;
using InnerNet;

namespace FungleAPI.Base.Rpc
{
    [FungleIgnore]
    public class AdvancedRpc<DataT> : BaseRpcHelper<InnerNetObject>
    {
        public void Send(DataT data, InnerNetObject innerNetObject, SendOption sendOption = SendOption.Reliable, int targetClientId = -1)
        {
            CustomRpcManager.SendRpc(innerNetObject, delegate (MessageWriter writer)
            {
                writer.WriteRPC(this);
                Write(innerNetObject, writer, data);
            }, sendOption, targetClientId);
        }
        public virtual void Write(MessageWriter messageWriter, DataT value)
        {
        }
        public virtual void Write(InnerNetObject innerNetObject, MessageWriter messageWriter, DataT value)
        {
            Write(messageWriter, value);
        }
    }
    [FungleIgnore]
    public class AdvancedRpc<DataT, TNetObject> : BaseRpcHelper<TNetObject> where TNetObject : InnerNetObject
    {
        public void Send(DataT data, TNetObject innerNetObject, SendOption sendOption = SendOption.Reliable, int targetClientId = -1)
        {
            CustomRpcManager.SendRpc(innerNetObject, delegate (MessageWriter writer)
            {
                writer.WriteRPC(this);
                Write(innerNetObject, writer, data);
            }, sendOption, targetClientId);
        }
        public virtual void Write(MessageWriter messageWriter, DataT value)
        {
        }
        public virtual void Write(TNetObject innerNetObject, MessageWriter messageWriter, DataT value)
        {
            Write(messageWriter, value);
        }
    }
}
