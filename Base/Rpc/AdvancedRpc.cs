using AmongUs.InnerNet.GameDataMessages;
using FungleAPI.Api;
using FungleAPI.Attributes;
using FungleAPI.Networking;
using FungleAPI.PluginLoading;
using FungleAPI.Utilities;
using Hazel;
using InnerNet;
using System;

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
            CustomRpcManager.SendRpc(innerNetObject, CustomRpcManager.DefaultRpc, delegate (MessageWriter writer)
            {
                writer.WriteRPC(this);
                writer.StartMessage(0);
                Write(innerNetObject, writer, data);
                writer.EndMessage();
            }, sendOption, targetClientId);
        }
        /// <summary>
        /// Send the Rpc
        /// </summary>
        public void SendLate(DataT data, InnerNetObject innerNetObject)
        {
            MessageWriter tempWriter = MessageWriter.Get(SendOption.Reliable);
            Write(innerNetObject, tempWriter, data);

            int headerOffset = 3;
            int payloadLength = tempWriter.Length - headerOffset;
            byte[] payload = new byte[payloadLength];
            Buffer.BlockCopy(tempWriter.Buffer, headerOffset, payload, 0, payloadLength);

            CustomRpcMessage customRpcMessage = CustomRpcMessage.CreateOne(innerNetObject.NetId, CustomRpcManager.DefaultRpc, delegate (MessageWriter writer)
            {
                writer.WriteRPC(this);
                writer.StartMessage(0);
                writer.Write(payload, 0, payload.Length);
                writer.EndMessage();
            });

            AmongUsClient.Instance.LateBroadcastReliableMessage(customRpcMessage.SafeCast<IGameDataMessage>());
        }
        /// <summary>
        /// Write the Rpc data into the message writer
        /// </summary>
        public virtual void Write(MessageWriter messageWriter, DataT data)
        {
        }
        /// <summary>
        /// Write the Rpc data with access to the related InnerNetObject
        /// </summary>
        public virtual void Write(InnerNetObject innerNetObject, MessageWriter messageWriter, DataT data)
        {
            Write(messageWriter, data);
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
            CustomRpcManager.SendRpc(innerNetObject, CustomRpcManager.DefaultRpc, delegate (MessageWriter writer)
            {
                writer.WriteRPC(this);
                writer.StartMessage(0);
                Write(innerNetObject, writer, data);
                writer.EndMessage();
            }, sendOption, targetClientId);
        }
        /// <summary>
        /// Send the Rpc
        /// </summary>
        public void SendLate(DataT data, TNetObject innerNetObject)
        {
            MessageWriter tempWriter = MessageWriter.Get(SendOption.Reliable);
            Write(innerNetObject, tempWriter, data);

            int headerOffset = 3;
            int payloadLength = tempWriter.Length - headerOffset;
            byte[] payload = new byte[payloadLength];
            Buffer.BlockCopy(tempWriter.Buffer, headerOffset, payload, 0, payloadLength);

            tempWriter.Recycle();

            CustomRpcMessage customRpcMessage = CustomRpcMessage.CreateOne(innerNetObject.NetId, CustomRpcManager.DefaultRpc, delegate (MessageWriter writer)
            {
                writer.WriteRPC(this);
                writer.StartMessage(0);
                writer.Write(payload, 0, payload.Length);
                writer.EndMessage();
            });

            AmongUsClient.Instance.LateBroadcastReliableMessage(customRpcMessage.SafeCast<IGameDataMessage>());
        }
        /// <summary>
        /// Write the Rpc data into the message writer
        /// </summary>
        public virtual void Write(MessageWriter messageWriter, DataT data)
        {
        }
        /// <summary>
        /// Write the Rpc data with access to the specific InnerNetObject type
        /// </summary>
        public virtual void Write(TNetObject innerNetObject, MessageWriter messageWriter, DataT data)
        {
            Write(messageWriter, data);
        }
    }
}