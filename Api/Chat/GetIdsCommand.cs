using FungleAPI.Chat;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace FungleAPI.Api.Chat
{
    internal class GetIdsCommand : BaseChatCommand
    {
        public override string CommandName => "id";
        public override void ExecuteCommand(IEnumerable<string> args, ref bool cancelSend)
        {
            HudManager.Instance.Chat.AddChatWarning($"{Color.black.ToTextColor()}{string.Join("\n", PlayerControl.AllPlayerControls.ToArray().Select(p => $"{p.Data.PlayerName}: {p.PlayerId}"))}</color>");

            cancelSend = true;
        }
    }
}
