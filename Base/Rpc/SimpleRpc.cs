using AmongUs.InnerNet.GameDataMessages;
using FungleAPI.Api;
using FungleAPI.Attributes;
using FungleAPI.Networking;
using FungleAPI.PluginLoading;
using FungleAPI.Utilities;
using Hazel;
using InnerNet;
using System;
using static Il2CppSystem.Globalization.CultureInfo;

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
            CustomRpcManager.SendRpc(innerNetObject, CustomRpcManager.DefaultRpc, delegate (MessageWriter writer)
            {
                writer.WriteRPC(this);
                writer.StartMessage(0);
                Write(innerNetObject, writer);
                writer.EndMessage();
            }, sendOption, targetClientId);
        }
        /// <summary>
        /// Send the Rpc
        /// </summary>
        public void SendLate(InnerNetObject innerNetObject)
        {
            MessageWriter tempWriter = MessageWriter.Get(SendOption.Reliable);
            Write(innerNetObject, tempWriter);

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
            CustomRpcManager.SendRpc(innerNetObject, CustomRpcManager.DefaultRpc, delegate (MessageWriter writer)
            {
                writer.WriteRPC(this);
                writer.StartMessage(0);
                Write(innerNetObject, writer);
                writer.EndMessage();
            }, sendOption, targetClientId);
        }
        /// <summary>
        /// Send the Rpc
        /// </summary>
        public void SendLate(TNetObject innerNetObject)
        {
            MessageWriter tempWriter = MessageWriter.Get(SendOption.Reliable);
            Write(innerNetObject, tempWriter);

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
        /// Write the RPC into the message writer
        /// </summary>
        public virtual void Write(MessageWriter messageWriter)
        {
        }
        /// <summary>
        /// Write the RPC with access to the specific InnerNetObject type
        /// </summary>
        public virtual void Write(TNetObject innerNetObject, MessageWriter messageWriter)
        {
            Write(messageWriter);
        }
    }
}