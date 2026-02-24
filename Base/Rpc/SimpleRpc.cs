using FungleAPI.Attributes;
using FungleAPI.Networking;
using Hazel;
using InnerNet;

namespace FungleAPI.Base.Rpc
{
    /// <summary>
    /// Base class to create a Rpc
    /// </summary>
    [FungleIgnore]
    public class SimpleRpc : BaseRpcHelper<InnerNetObject>
    {
        /// <summary>
        /// Send the Rpc
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
        /// Write the Rpc into the message writer
        /// </summary>
        public virtual void Write(MessageWriter messageWriter)
        {
        }
        /// <summary>
        /// Write the Rpc with access to the related InnerNetObject
        /// </summary>
        public virtual void Write(InnerNetObject innerNetObject, MessageWriter messageWriter)
        {
            Write(messageWriter);
        }
    }
    /// <summary>
    /// Base class to create a Rpc
    /// </summary>
    [FungleIgnore]
    public class SimpleRpc<TNetObject> : BaseRpcHelper<TNetObject> where TNetObject : InnerNetObject
    {
        /// <summary>
        /// Send the Rpc
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
        /// Write the Rpc into the message writer
        /// </summary>
        public virtual void Write(MessageWriter messageWriter)
        {
        }
        /// <summary>
        /// Write the Rpc with access to the specific InnerNetObject type
        /// </summary>
        public virtual void Write(TNetObject innerNetObject, MessageWriter messageWriter)
        {
            Write(messageWriter);
        }
    }
}