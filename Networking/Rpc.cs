using FungleAPI.Base.Rpc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FungleAPI.Networking
{
    public static class Rpc<TRpc> where TRpc : RpcHelper
    {
        public static TRpc Instance => CustomRpcManager.Instance<TRpc>();
    }
}
