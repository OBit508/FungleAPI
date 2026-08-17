using FungleAPI.Chat;
using FungleAPI.Networking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FungleAPI.Api.Chat
{
    internal class BanCommand : BaseChatCommand
    {
        public override string CommandName => "ban";
        public override string[] Arguments { get; } = new string[] { "id" };
        public override void ExecuteCommand(IEnumerable<string> args, ref bool cancelSend)
        {
            cancelSend = true;

            if (!AmongUsClient.Instance.AmHost) return;

            if (args.Count() == 1)
            {
                if (byte.TryParse(args.ElementAt(0), out byte result))
                {
                    int clientId = GameData.Instance.GetPlayerById(result)?.ClientId ?? int.MinValue;

                    if (clientId == int.MinValue) return;

                    AmongUsClient.Instance.KickPlayer(clientId, true);
                }
            }
        }
    }
}
