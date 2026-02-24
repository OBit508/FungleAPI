using FungleAPI.Attributes;
using FungleAPI.Networking;
using Hazel;
using InnerNet;

namespace FungleAPI.Base.Rpc
{
    /// <summary>
    /// Base class to create a Rpc
    /// </summary>
    /// <typeparam name="TData">The given type to the Write</typeparam>
    [FungleIgnore]
    public class AdvancedRpc<DataT> : BaseRpcHelper<InnerNetObject>
    {
        /// <summary>
        /// Send the Rpc
        /// </summary>
        public void Send(DataT data, InnerNetObject innerNetObject, SendOption sendOption = SendOption.Reliable, int targetClientId = -1)
        {
            CustomRpcManager.SendRpc(innerNetObject, delegate (MessageWriter writer)
            {
                writer.WriteRPC(this);
                writer.StartMessage(0);
                Write(innerNetObject, writer, data);
                writer.EndMessage();
            }, sendOption, targetClientId);
        }
        /// <summary>
        /// Write the Rpc data into the message writer
        /// </summary>
        public virtual void Write(MessageWriter messageWriter, DataT value)
        {
        }
        /// <summary>
        /// Write the Rpc data with access to the related InnerNetObject
        /// </summary>
        public virtual void Write(InnerNetObject innerNetObject, MessageWriter messageWriter, DataT value)
        {
            Write(messageWriter, value);
        }
    }
    /// <summary>
    /// Base class to create a Rpc
    /// </summary>
    /// <typeparam name="TData">The given type to the Write</typeparam>
    /// <typeparam name="TNetObject">The InnerNetObject Type</typeparam>
    [FungleIgnore]
    public class AdvancedRpc<DataT, TNetObject> : BaseRpcHelper<TNetObject> where TNetObject : InnerNetObject
    {
        /// <summary>
        /// Send the Rpc
        /// </summary>
        public void Send(DataT data, TNetObject innerNetObject, SendOption sendOption = SendOption.Reliable, int targetClientId = -1)
        {
            CustomRpcManager.SendRpc(innerNetObject, delegate (MessageWriter writer)
            {
                writer.WriteRPC(this);
                writer.StartMessage(0);
                Write(innerNetObject, writer, data);
                writer.EndMessage();
            }, sendOption, targetClientId);
        }
        /// <summary>
        /// Write the Rpc data into the message writer
        /// </summary>
        public virtual void Write(MessageWriter messageWriter, DataT value)
        {
        }
        /// <summary>
        /// Write the Rpc data with access to the specific InnerNetObject type
        /// </summary>
        public virtual void Write(TNetObject innerNetObject, MessageWriter messageWriter, DataT value)
        {
            Write(messageWriter, value);
        }
    }
}