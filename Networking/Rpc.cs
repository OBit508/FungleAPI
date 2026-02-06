using FungleAPI.Base.Rpc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FungleAPI.Networking
{
    /// <summary>
    /// 
    /// </summary>
    public static class Rpc<TRpc> where TRpc : RpcHelper
    {
        /// <summary>
        /// 
        /// </summary>
        public static TRpc Instance => CustomRpcManager.GetRpcInstance<TRpc>();
    }
}
