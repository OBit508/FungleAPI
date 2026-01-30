using FungleAPI.Attributes;
using FungleAPI.PluginLoading;
using FungleAPI.Utilities;
using Hazel;
using InnerNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FungleAPI.Base.Rpc
{
    [FungleIgnore]
    public class RpcHelper
    {
        public int RpcId;
        public virtual void __handle(InnerNetObject innerNetObject, MessageReader messageReader)
        {
        }
    }
    [FungleIgnore]
    public class BaseRpcHelper<TNetObject> : RpcHelper where TNetObject : InnerNetObject
    {
        public virtual void Handle(MessageReader messageReader)
        {
        }
        public virtual void Handle(TNetObject innerNetObject, MessageReader messageReader)
        {
            Handle(messageReader);
        }
        public sealed override void __handle(InnerNetObject innerNetObject, MessageReader messageReader)
        {
            TNetObject netObject = innerNetObject.SafeCast<TNetObject>();
            if (netObject != null)
            {
                Handle(netObject, messageReader);
            }
            else
            {
                FungleAPIPlugin.Instance.Log.LogError(GetType().Name + "-Rpc is trying to handle with the wrong InnerNetObject");
            }
        }
    }
}
