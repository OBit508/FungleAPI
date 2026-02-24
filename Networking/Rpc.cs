using FungleAPI.Base.Rpc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FungleAPI.Networking
{
    /// <summary>
    /// A class to easily get the instance of a given type
    /// </summary>
    public static class Rpc<TRpc> where TRpc : RpcHelper
    {
        /// <summary>
        /// The instance
        /// </summary>
        public static TRpc Instance => CustomRpcManager.GetRpcInstance<TRpc>();
    }
}
