using AmongUs.GameOptions;
using AmongUs.InnerNet.GameDataMessages;
using FungleAPI.Api;
using FungleAPI.Attributes;
using FungleAPI.Base.Rpc;
using FungleAPI.PluginLoading;
using FungleAPI.Utilities;
using Hazel;
using Il2CppInterop.Generator.Extensions;
using Il2CppInterop.Runtime;
using InnerNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Il2CppSystem.Globalization.CultureInfo;

namespace FungleAPI.Networking
{
    internal class CustomRpcMessage : Il2CppSystem.Object
    {
        public GameDataTypes GameDataType => GameDataTypes.RpcFlag;
        public byte CallId;
        public Action<MessageWriter> Write;
        public uint NetObject;
        public CustomRpcMessage(IntPtr ptr) : base(ptr) { }
        public void Serialize(MessageWriter msg)
        {
            msg.StartMessage((byte)GameDataType);
            msg.WritePacked(NetObject);
            msg.Write(CallId);
            Write(msg);
            msg.EndMessage();
        }
        public static CustomRpcMessage CreateOne(uint netObject, byte callId, Action<MessageWriter> writeValues)
        {
            CustomRpcMessage customRpcMessage = Il2CppSystem.Activator.CreateInstance(Il2CppType.Of<CustomRpcMessage>()).SafeCast<CustomRpcMessage>();
            customRpcMessage.CallId = callId;
            customRpcMessage.NetObject = netObject;
            customRpcMessage.Write = writeValues;
            return customRpcMessage;
        }
    }
}
