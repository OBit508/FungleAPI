using FungleAPI.Api;
using FungleAPI.Networking;
using InnerNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FungleAPI.AntiCheat
{
    public static class AntiCheatManager
    {
        public static bool Enabled = true;
        public static CheatFlags FindCheater = CheatFlags.WarnHost | CheatFlags.Kick;

        public static bool Active => AmongUsClient.Instance.AmHost && Enabled;

        public static void CheaterFinded(int clientId)
        {
            AmongUsClient amongUsClient = AmongUsClient.Instance;

            ClientData clientData = amongUsClient.FindClientById(clientId);

            bool warnEveryone = FindCheater.HasFlag(CheatFlags.WarnEveryone);

            if (FindCheater.HasFlag(CheatFlags.WarnHost) || warnEveryone)
            {
                HudManager.Instance.Notifier.AddDisconnectMessage(string.Format(FungleTranslation.CheatingWarnText.GetString(), clientData.PlayerName));

                if (warnEveryone)
                {
                    Rpc<RpcWarnFromCheater>.Instance.Send(clientData.PlayerName);
                }
            }

            if (FindCheater.HasFlag(CheatFlags.Kick))
            {
                amongUsClient.KickPlayer(clientId, false);
                return;
            }

            if (FindCheater.HasFlag(CheatFlags.Ban))
            {
                amongUsClient.KickPlayer(clientId, true);
            }
        }
    }
}
