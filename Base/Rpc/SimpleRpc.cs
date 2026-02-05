using FungleAPI.Attributes;
using FungleAPI.Networking;
using Hazel;
using InnerNet;

namespace FungleAPI.Base.Rpc
{
    /// <summary>
    /// 
    /// </summary>
    [FungleIgnore]
    public class SimpleRpc : BaseRpcHelper<InnerNetObject>
    {
        /// <summary>
        /// 
        /// </summary>
        public void Send(InnerNetObject innerNetObject, SendOption sendOption = SendOption.Reliable, int targetClientId = -1)
        {
            CustomRpcManager.SendRpc(innerNetObject, delegate (MessageWriter writer)
            {
                writer.WriteRPC(this);
                writer.StartMessage(0);
                Write(innerNetObject, writer);
                writer.EndMessage();
            }, sendOption, targetClientId);
        }
        /// <summary>
        /// 
        /// </summary>
        public virtual void Write(MessageWriter messageWriter)
        {
        }
        /// <summary>
        /// 
        /// </summary>
        public virtual void Write(InnerNetObject innerNetObject, MessageWriter messageWriter)
        {
            Write(messageWriter);
        }
    }
    /// <summary>
    /// 
    /// </summary>
    [FungleIgnore]
    public class SimpleRpc<TNetObject> : BaseRpcHelper<TNetObject> where TNetObject : InnerNetObject
    {
        /// <summary>
        /// 
        /// </summary>
        public void Send(TNetObject innerNetObject, SendOption sendOption = SendOption.Reliable, int targetClientId = -1)
        {
            CustomRpcManager.SendRpc(innerNetObject, delegate (MessageWriter writer)
            {
                writer.WriteRPC(this);
                writer.StartMessage(0);
                Write(innerNetObject, writer);
                writer.EndMessage();
            }, sendOption, targetClientId);
        }
        /// <summary>
        /// 
        /// </summary>
        public virtual void Write(MessageWriter messageWriter)
        {
        }
        /// <summary>
        /// 
        /// </summary>
        public virtual void Write(TNetObject innerNetObject, MessageWriter messageWriter)
        {
            Write(messageWriter);
        }
    }
}
