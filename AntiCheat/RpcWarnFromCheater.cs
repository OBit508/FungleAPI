using FungleAPI.Api;
using FungleAPI.Base.Rpc;
using Hazel;
using InnerNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FungleAPI.AntiCheat
{
    internal class RpcWarnFromCheater : AdvancedRpc<string, PlayerControl>
    {
        public override void Write(MessageWriter messageWriter, string data)
        {
            messageWriter.Write(data);
        }
        public override void Handle(PlayerControl innerNetObject, MessageReader messageReader)
        {
            if (innerNetObject == null) return;

            if (AntiCheatManager.Active && !innerNetObject.AmOwner)
            {
                AntiCheatManager.CheaterFinded(innerNetObject.Data.ClientId);

                return;
            }

            string playerName = messageReader.ReadString();
            HudManager.Instance.Notifier.AddDisconnectMessage(string.Format(FungleTranslation.CheatingWarnText.GetString(), playerName));
        }
    }
}
