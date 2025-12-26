using FungleAPI.Attributes;
using FungleAPI.Networking;
using Hazel;
using InnerNet;

namespace FungleAPI.Base.Rpc
{
    [FungleIgnore]
    public class SimpleRpc : BaseRpcHelper<InnerNetObject>
    {
        public void Send(InnerNetObject innerNetObject, SendOption sendOption = SendOption.Reliable, int targetClientId = -1)
        {
            CustomRpcManager.SendRpc(innerNetObject, delegate (MessageWriter writer)
            {
                writer.WriteRPC(this);
                Write(innerNetObject, writer);
            }, sendOption, targetClientId);
        }
        public virtual void Write(MessageWriter messageWriter)
        {
        }
        public virtual void Write(InnerNetObject innerNetObject, MessageWriter messageWriter)
        {
            Write(messageWriter);
        }
    }
    [FungleIgnore]
    public class SimpleRpc<TNetObject> : BaseRpcHelper<TNetObject> where TNetObject : InnerNetObject
    {
        public void Send(TNetObject innerNetObject, SendOption sendOption = SendOption.Reliable, int targetClientId = -1)
        {
            CustomRpcManager.SendRpc(innerNetObject, delegate (MessageWriter writer)
            {
                writer.WriteRPC(this);
                Write(innerNetObject, writer);
            }, sendOption, targetClientId);
        }
        public virtual void Write(MessageWriter messageWriter)
        {
        }
        public virtual void Write(TNetObject innerNetObject, MessageWriter messageWriter)
        {
            Write(messageWriter);
        }
    }
}
