using FungleAPI.Networking.RPCs;
using Hazel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace FungleAPI.Networking
{
    public class RpcPair
    {
        public MessageWriter Writer;
        internal int id;
        internal Action SendAll;
        internal RpcPair()
        {
        }
        public void AddRpc<TData>(CustomRpc<TData> rpc, TData data)
        {
            SendAll += delegate
            {
                Type type = rpc.GetType();
                Writer.Write(ModPlugin.GetModPlugin(type.Assembly).ModName);
                Writer.Write(type.FullName);
                rpc.Write(Writer, data);
            };
            id++;
        }
        public void SendPair()
        {
            Writer.Write(id);
            SendAll();
            AmongUsClient.Instance.FinishRpcImmediately(Writer);
        }
    }
}
