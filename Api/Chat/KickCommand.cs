using FungleAPI.Chat;
using FungleAPI.Networking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FungleAPI.Api.Chat
{
    internal class KickCommand : BaseChatCommand
    {
        public override string CommandName => "kick";
        public override string[] Arguments { get; } = new string[] { "id", "reason" };
        public override void ExecuteCommand(IEnumerable<string> args, ref bool cancelSend)
        {
            cancelSend = true;

            if (!AmongUsClient.Instance.AmHost) return;

            if (args.Count() >= 1)
            {
                if (byte.TryParse(args.ElementAt(0), out byte result))
                {
                    int clientId = GameData.Instance.GetPlayerById(result)?.ClientId ?? int.MinValue;

                    if (clientId == int.MinValue) return;

                    if (args.Count() > 1)
                    {
                        AntiCheatManager.KickWithReason(clientId, string.Join(" ", args.Skip(1)));
                        return;
                    }
                    AmongUsClient.Instance.KickPlayer(clientId, false);
                }
            }
        }
    }
}
