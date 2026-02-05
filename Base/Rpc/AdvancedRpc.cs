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
    public class AdvancedRpc<DataT> : BaseRpcHelper<InnerNetObject>
    {
        /// <summary>
        /// 
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
        /// 
        /// </summary>
        public virtual void Write(MessageWriter messageWriter, DataT value)
        {
        }
        /// <summary>
        /// 
        /// </summary>
        public virtual void Write(InnerNetObject innerNetObject, MessageWriter messageWriter, DataT value)
        {
            Write(messageWriter, value);
        }
    }
    /// <summary>
    /// 
    /// </summary>
    [FungleIgnore]
    public class AdvancedRpc<DataT, TNetObject> : BaseRpcHelper<TNetObject> where TNetObject : InnerNetObject
    {
        /// <summary>
        /// 
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
        /// 
        /// </summary>
        public virtual void Write(MessageWriter messageWriter, DataT value)
        {
        }
        /// <summary>
        /// 
        /// </summary>
        public virtual void Write(TNetObject innerNetObject, MessageWriter messageWriter, DataT value)
        {
            Write(messageWriter, value);
        }
    }
}
