using FungleAPI.Base.Rpc;
using Hazel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FungleAPI.Networking
{
    internal class RpcKickWithReason : AdvancedRpc<(string, int), PlayerControl>
    {
        public override void Write(MessageWriter messageWriter, (string, int) data)
        {
            messageWriter.Write(data.Item1);
            messageWriter.WritePacked(data.Item2);
        }
        public override void Handle(PlayerControl innerNetObject, MessageReader messageReader)
        {
            if (!AntiCheatManager.CheckForCheater(innerNetObject)) return;

            string reason = messageReader.ReadString();
            int clientId = messageReader.ReadPackedInt32();

            if (AmongUsClient.Instance.ClientId == clientId)
            {
                string name = null;
                if (reason.Contains("{0}"))
                {
                    name = "You";
                }
                AntiCheatManager.LastKickReason = name != null ? string.Format(reason, name) : reason;
            }
            else
            {
                string name = null;
                if (reason.Contains("{0}"))
                {
                    name = AmongUsClient.Instance.GetClient(clientId).PlayerName;
                }
                HudManager.Instance?.Notifier.AddDisconnectMessage(name != null ? string.Format(reason, name) : reason);
            }
        }
    }
}
