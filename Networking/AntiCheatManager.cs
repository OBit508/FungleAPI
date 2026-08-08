using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FungleAPI.Networking
{
    public static class AntiCheatManager
    {
        public static string LastKickReason;
        public static bool IsHost(this PlayerControl playerControl)
        {
            return AmongUsClient.Instance.HostId == playerControl.OwnerId;
        }
        public static void KickWithReason(int clientId, string reason)
        {
            if (!AmongUsClient.Instance.AmHost) return;

            Rpc<RpcKickWithReason>.Instance.Send((reason, clientId), PlayerControl.LocalPlayer);
            string name = null;
            if (reason.Contains("{0}"))
            {
                name = AmongUsClient.Instance.GetClient(clientId).PlayerName;
            }
            HudManager.Instance?.Notifier.AddDisconnectMessage(name != null ? string.Format(reason, name) : reason);
            AmongUsClient.Instance.KickPlayer(clientId, false);
        }
        public static bool CheckForCheater(PlayerControl playerControl, string kickReason = "{0} was caught cheating.")
        {
            if (!playerControl.IsHost())
            {
                KickWithReason(playerControl.OwnerId, kickReason);
                return false;
            }
            return true;
        }
    }
}
