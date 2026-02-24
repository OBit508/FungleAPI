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
    /// <summary>
    /// The rpc base class helper
    /// </summary>
    [FungleIgnore]
    public class RpcHelper
    {
        /// <summary>
        /// The Rpc identifier.
        /// </summary>
        public int RpcId;
        internal virtual void __handle(InnerNetObject innerNetObject, MessageReader messageReader)
        {
        }
    }
    /// <summary>
    /// The rpc base class
    /// </summary>
    /// <typeparam name="TNetObject">The InnerNetObject Type</typeparam>
    [FungleIgnore]
    public class BaseRpcHelper<TNetObject> : RpcHelper where TNetObject : InnerNetObject
    {
        /// <summary>
        /// Handle the Rpc without a specific InnerNetObject.
        /// </summary>
        public virtual void Handle(MessageReader messageReader)
        {
        }
        /// <summary>
        /// Handle the Rpc with the specific InnerNetObject type.
        /// </summary>
        public virtual void Handle(TNetObject innerNetObject, MessageReader messageReader)
        {
            Handle(messageReader);
        }
        internal override void __handle(InnerNetObject innerNetObject, MessageReader messageReader)
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