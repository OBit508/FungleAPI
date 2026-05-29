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
        private static TRpc __rpc;
        /// <summary>
        /// The instance
        /// </summary>
        public static TRpc Instance
        {
            get
            {
                if (__rpc == null)
                {
                    __rpc = CustomRpcManager.GetRpcInstance<TRpc>();
                }
                return __rpc;
            }
        }
    }
}
